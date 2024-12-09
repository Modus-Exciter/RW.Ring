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
    private bool m_filtering;
    private bool m_removing_filter;
    private bool m_auto_resizing;
    private readonly List<FilterCell> m_filter_row = new List<FilterCell>();

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

      m_auto_resizing = true;

      if (m_grid.Rows.Count < (1 << 10))
        m_grid.AutoResizeColumns();
      else
        m_grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

      m_grid.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader);

      m_auto_resizing = false;

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

    private void HandleFilterTextChanged(object sender, EventArgs e)
    {
      var text_box = sender as FilterCell;

      if (text_box == null || m_removing_filter)
        return;

      this.ApplyRowFilter();
    }

    private void Switcher_LanguageChanged(object sender, Notung.ComponentModel.LanguageEventArgs e)
    {
      m_cmd_tbedit.Text = Resources.TABLE_EDIT;
      m_tool_tip.SetToolTip(this.m_close_filter_button, Resources.DISABLE_FILTER);
      m_tool_tip.SetToolTip(this.m_unsort_button, Resources.DISABLE_SORTING);
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

    private void HandleGridScroll(object sender, ScrollEventArgs e)
    {
      if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
        this.UpdateAutoFilterColumnWidths();
    }

    private void HandleGridDataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      AppManager.Notificator.Show(e.Exception.Message, InfoLevel.Error);
    }

    private void HandleGridSorted(object sender, EventArgs e)
    {
      this.UpdateBottomPanel();
      this.UpdateAutoFilterColumnWidths();
    }

    private void HandleUnsortClick(object sender, EventArgs e)
    {
      this.DataSource.DefaultView.Sort = string.Empty;
      this.UpdateBottomPanel();
    }

    private void HandleCloseFilterButtonClick(object sender, EventArgs e)
    {
      m_removing_filter = true;

      try
      {
        foreach (DataGridViewColumn column in m_grid.Columns)
          ((FilterCell)column.Tag).Text = string.Empty;
      }
      finally
      {
        m_removing_filter = false;
      }

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

      m_grid.Parent.SuspendLayout();
      m_filter_panel.Visible = false;
      m_filter_label.Text = string.Empty;
      m_filter_row.Clear();

      foreach (DataGridViewColumn column in m_grid.Columns)
      {
        var cell = new FilterCell(column);

        cell.TextChanged += this.HandleFilterTextChanged;

        column.Tag = cell;
        m_filter_row.Add(cell);
        m_grid.Parent.Controls.Add(cell);
        m_grid.Parent.Controls.SetChildIndex(cell, m_grid.Parent.Controls.GetChildIndex(m_grid));
      }

      m_grid.Parent.ResumeLayout();
      m_grid.Parent.PerformLayout();
    }

    private void UpdateAutoFilterColumnWidths()
    {
      if (m_auto_resizing)
        return;

      for (int i = 0; i < m_grid.ColumnCount; i++)
      {
        var cell = m_grid.Columns[i].Tag as FilterCell;

        if (cell == null) 
          return;

        // Позиционирование поля
        Rectangle headerRect = m_grid.GetCellDisplayRectangle(i, -1, true);
        cell.Visible = i >= m_grid.FirstDisplayedScrollingColumnIndex;
        cell.Location = new Point(headerRect.X + 3, 
          headerRect.Height - m_grid.ColumnHeadersDefaultCellStyle.Padding.Bottom + 2);
        cell.Size = new Size(headerRect.Width - 6, cell.Size.Height);
      }
    }

    private IEnumerable<KeyValuePair<string, string>> GetFilterCondition()
    {
      return m_filter_row.Where(cell => !string.IsNullOrEmpty(cell.Text))
        .Select(cell => new KeyValuePair<string, string>(cell.Property, cell.Text));
    }

    private void ApplyRowFilter()
    {
      m_filter_label.Text = string.Join(" && ", this.GetFilterCondition().Select(kv =>
        string.Format("{0} ≈ '{1}'", kv.Key, kv.Value)));

      m_close_filter_button.Visible = !string.IsNullOrEmpty(m_filter_label.Text);
      this.UpdateBottomPanel();
      m_filtering = true;

      try
      {
        this.DataSource.DefaultView.RowFilter = string.Join(" AND ", this.GetFilterCondition().Select(kv =>
          string.Format("Convert([{0}], 'System.String') LIKE '{1}%'", kv.Key, kv.Value)));
      }
      finally
      {
        m_filtering = false;
      }

      this.UpdateAutoFilterColumnWidths();
    }

    private void UpdateBottomPanel()
    {
      m_unsort_button.Visible = !string.IsNullOrEmpty(this.DataSource.DefaultView.Sort);
      m_separator.Visible = m_close_filter_button.Visible;
      m_filter_panel.Visible = !string.IsNullOrEmpty(m_filter_label.Text)
       || !string.IsNullOrEmpty(this.DataSource.DefaultView.Sort);
    }

    #endregion
  }
}