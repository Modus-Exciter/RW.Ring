using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Notung;
using Notung.Services;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm
{
  public partial class TableForm : Form
  {
    private readonly List<TextBox> m_filter_row = new List<TextBox>();
    private readonly Dictionary<string, string> m_filter_condition = new Dictionary<string, string>();
    private bool m_filtering;
    private bool m_removing_filter;
    private bool m_auto_resizing;

    public TableForm()
    {
      this.InitializeComponent();
    }

    #region Properties ----------------------------------------------------------------------------

    public DataTable DataSource
    {
      get { return m_grid.DataSource as DataTable; }
      set
      {
        if (ReferenceEquals(value, m_grid.DataSource))
          return;

        m_grid.DataSource = value;

        if (value != null)
        {
          value.AcceptChanges();

          if (m_grid.ReadOnly)
          {
            foreach (DataGridViewColumn col in m_grid.Columns)
            {
              if (col.ValueType == typeof(double) || col.ValueType == typeof(float))
                col.DefaultCellStyle.Format = "0.000";
            }
          }

          this.CreateFilterRow();
        }
      }
    }

    public string FileName { get; set; }

    #endregion

    #region Public methods ------------------------------------------------------------------------

    public void MarkAsReadOnly()
    {
      m_grid.ReadOnly = true;
      m_grid.AllowUserToAddRows = false;
      m_grid.AllowUserToDeleteRows = false;
    }

    public void Save()
    {
      if (string.IsNullOrEmpty(this.FileName))
        this.SaveAs();
      else
        this.SaveFile(this.FileName, this.DataSource);
    }

    public void SaveAs()
    {
      using (var dlg = new SaveFileDialog())
      {
        dlg.Filter = "Schicksal data files|*.sks";

        if (!string.IsNullOrEmpty(this.FileName))
        {
          dlg.FileName = Path.GetFileName(this.FileName);
          dlg.InitialDirectory = Path.GetDirectoryName(this.FileName);
        }

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          this.FileName = dlg.FileName;
          this.SaveFile(this.FileName, this.DataSource);
        }
      }
    }

    #endregion

    #region Form handling overrides ---------------------------------------------------------------

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.Icon = this.Icon.Clone() as Icon;
    }

    protected override void OnShown(System.EventArgs e)
    {
      base.OnShown(e);

      if (m_grid.Rows.Count < (1 << 10))
      {
        m_auto_resizing = true;
        m_grid.AutoResizeColumns();
        m_auto_resizing = false;
      }

      this.UpdateAutoFilterColumnWidths();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      if (this.DataSource != null && this.DataSource.GetChanges() != null)
      {
        var res = AppManager.Notificator.ConfirmOrCancel(string.Format(
          Resources.SAVE_FILE_CONFIRM, this.Text.Substring(0, this.Text.Length - 1)), InfoLevel.Info);

        if (res == null)
          e.Cancel = true;

        else if (res == true)
        {
          this.Save();

          var parent = this.MdiParent as MainForm;

          if (parent != null)
            parent.FillLastFilesMenu();
        }
      }
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);

      this.UpdateAutoFilterColumnWidths();
    }

    #endregion

    #region Event handlers ------------------------------------------------------------------------

    private void HadnleFilterTextChanged(object sender, EventArgs e)
    {
      var text_box = sender as TextBox;

      if (text_box == null || m_removing_filter)
        return;

      if (!string.IsNullOrEmpty(text_box.Text))
        m_filter_condition[(string)text_box.Tag] = text_box.Text;
      else
        m_filter_condition.Remove((string)text_box.Tag);

      this.ApplyRowFilter();
    }

    private void Switcher_LanguageChanged(object sender, Notung.ComponentModel.LanguageEventArgs e)
    {
      m_cmd_tbedit.Text = Resources.TABLE_EDIT;
    }

    private void HandleContextMenuOpening(object sender, CancelEventArgs e)
    {
      if (m_grid.ReadOnly)
        e.Cancel = true;
    }

    private void HandleEditTableClick(object sender, EventArgs e)
    {
      using (var dlg = new EditColumnsDialog())
      {
        dlg.Text = Resources.TABLE_EDIT;
        TableColumnInfo.FillColumnInfo(dlg.Columns, this.DataSource);

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          m_grid.DataSource = TableColumnInfo.CreateUpdatedTable(dlg.Columns, this.DataSource);

          this.CreateFilterRow();
          this.UpdateAutoFilterColumnWidths();

          if (!this.Text.EndsWith("*"))
            this.Text += "*";
        }
      }
    }

    private void HandleGridCellEnter(object sender, DataGridViewCellEventArgs e)
    {
      if (!m_filtering)
        m_grid.BeginEdit(true);
    }

    private void HandleGridMouseClick(object sender, MouseEventArgs e)
    {
      DataGridView.HitTestInfo info = m_grid.HitTest(e.X, e.Y);
      DataGridViewCell cell = null;

      if (info != null && info.ColumnIndex >= 0 && info.RowIndex >= 0)
        cell = m_grid.Rows[info.RowIndex].Cells[info.ColumnIndex];

      if (m_grid.CurrentCell != null && m_grid.CurrentCell != cell)
      {
        if (m_grid.CurrentCell.IsInEditMode)
          m_grid.EndEdit();
        else
          m_grid.BeginEdit(false);
      }
    }

    private void HandleGridCellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || m_filtering)
        return;

      var row = m_grid.Rows[e.RowIndex].DataBoundItem as DataRowView;

      if (row == null)
        return;

      if (row.Row.RowState != DataRowState.Unchanged && !this.Text.EndsWith("*"))
        this.Text += "*";
    }

    private void HandleGridUserDeletedRow(object sender, DataGridViewRowEventArgs e)
    {
      if(!this.Text.EndsWith("*"))
        this.Text += "*";
    }

    private void HandleGridColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
    {
      this.UpdateAutoFilterColumnWidths();
    }

    private void HandleGridRowHeadersWidthChanged(object sender, EventArgs e)
    {
      this.UpdateAutoFilterColumnWidths();
    }

    private void HandleGridScroll(object sender, ScrollEventArgs e)
    {
      if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
        this.UpdateAutoFilterColumnWidths();
    }

    private void HandleGridDataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      AppManager.Notificator.Show(e.Exception.Message, InfoLevel.Error);
    }

    private void HandleCloseFilterButtonClick(object sender, EventArgs e)
    {
      m_removing_filter = true;

      try
      {
        foreach (var tb in m_filter_row)
          tb.Text = string.Empty;
      }
      finally
      {
        m_removing_filter = false;
      }

      m_filter_condition.Clear();
      this.ApplyRowFilter();
    }

    #endregion

    #region Implementation ------------------------------------------------------------------------

    private void SaveFile(string fileName, DataTable table)
    {
      if (File.Exists(fileName))
        File.Delete(fileName);

      table.AcceptChanges();

      using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      {
        DataTableSaver.WriteDataTable(table, fs);
      }

      AppManager.Configurator.GetSection<Program.Preferences>().LastFiles[fileName] = DateTime.Now;

      this.FileName = Path.GetFileName(this.FileName);
      this.Text = Path.GetFileName(this.FileName);
    }

    private void CreateFilterRow()
    {
      foreach (var tb in m_filter_row)
      {
        this.Controls.Remove(tb);
        tb.Dispose();
      }

      m_filter_row.Clear();
      m_filter_condition.Clear();
      m_grid.Parent.SuspendLayout();
      m_filter_panel.Visible = false;
      m_filter_label.Text = string.Empty;

      foreach (DataGridViewColumn column in m_grid.Columns)
      {
        var cell = new TextBox();

        cell.Tag = column.DataPropertyName;
        cell.TextChanged += this.HadnleFilterTextChanged;

        m_filter_row.Add(cell);
        m_grid.Parent.Controls.Add(cell);
        m_grid.Parent.Controls.SetChildIndex(cell, 0);
      }

      m_grid.Parent.ResumeLayout();
      m_grid.Parent.PerformLayout();
    }

    private void UpdateAutoFilterColumnWidths()
    {
      if (m_auto_resizing)
        return;

      for (int i = 0; i < m_filter_row.Count; i++)
      {
        var textBox = m_filter_row[i];

        // Позиционирование поля
        Rectangle headerRect = m_grid.GetCellDisplayRectangle(i, -1, true);
        textBox.Visible = i >= m_grid.FirstDisplayedScrollingColumnIndex;
        textBox.Location = new Point(headerRect.X + 3, headerRect.Height - m_grid.ColumnHeadersDefaultCellStyle.Padding.Bottom);
        textBox.Size = new Size(headerRect.Width - 6, headerRect.Height);
      }
    }

    private void ApplyRowFilter()
    {
      m_filter_label.Text = m_filter_condition.Count > 0 ? string.Join(" AND ",
        m_filter_condition.Select(kv => string.Format("Convert([{0}], 'System.String') LIKE '{1}%'", kv.Key, kv.Value))) : null;

      m_filter_panel.Visible = !string.IsNullOrEmpty(m_filter_label.Text);
      m_filtering = true;

      try
      {
        this.DataSource.DefaultView.RowFilter = m_filter_label.Text;
      }
      finally
      {
        m_filtering = false;
      }

      this.UpdateAutoFilterColumnWidths();
    }

    #endregion
  }
}