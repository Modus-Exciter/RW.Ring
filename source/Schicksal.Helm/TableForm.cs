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
    private bool m_auto_resizing;

    public TableForm()
    {
      this.InitializeComponent();
    }

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

    public void MarkAsReadOnly()
    {
      m_grid.ReadOnly = true;
      m_grid.AllowUserToAddRows = false;
      m_grid.AllowUserToDeleteRows = false;
    }

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

      this.UpdateAutoFilterColumnWidths(0);
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

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          this.FileName = dlg.FileName;
          this.SaveFile(this.FileName, this.DataSource);
        }
      }
    }

    private void CreateFilterRow()
    {
      foreach (var tb in m_filter_row)
      {
        this.Controls.Remove(tb);
        tb.Dispose();
      }

      m_filter_row.Clear();
      m_grid.Parent.SuspendLayout();

      foreach (DataGridViewColumn column in m_grid.Columns)
      {
        var cell = new TextBox();

        cell.Tag = column.DataPropertyName;
        cell.TextChanged += this.Cell_TextChanged;

        m_filter_row.Add(cell);
        m_grid.Parent.Controls.Add(cell);
        m_grid.Parent.Controls.SetChildIndex(cell, 0);
      }

      m_grid.Parent.ResumeLayout();
      m_grid.Parent.PerformLayout();
    }

    private void Cell_TextChanged(object sender, EventArgs e)
    {
      var text_box = sender as TextBox;

      if (text_box == null)
        return;

      if (!string.IsNullOrEmpty(text_box.Text))
        m_filter_condition[(string)text_box.Tag] = text_box.Text;
      else
        m_filter_condition.Remove((string)text_box.Tag);

      m_filtering = true;
      try
      {
        this.DataSource.DefaultView.RowFilter = m_filter_condition.Count > 0 ? string.Join(" AND ",
          m_filter_condition.Select(kv => string.Format("Convert([{0}], 'System.String') LIKE '{1}%'", kv.Key, kv.Value))) : null;
      }
      finally
      {
        m_filtering = false;
      }
    }

    private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      AppManager.Notificator.Show(e.Exception.Message, InfoLevel.Error);
    }

    private void Switcher_LanguageChanged(object sender, Notung.ComponentModel.LanguageEventArgs e)
    {
      m_cmd_tbedit.Text = Resources.TABLE_EDIT;
    }

    private void m_cmd_tbedit_Click(object sender, EventArgs e)
    {
      using (var dlg = new EditColumnsDialog())
      {
        dlg.Text = Resources.TABLE_EDIT;
        TableColumnInfo.FillColumnInfo(dlg.Columns, this.DataSource);

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          m_grid.DataSource = TableColumnInfo.CreateUpdatedTable(dlg.Columns, this.DataSource);

          if (!this.Text.EndsWith("*"))
            this.Text += "*";
        }
      }
    }

    private void m_grid_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
      if (m_filtering) 
        return;

      m_grid.BeginEdit(true);
    }

    private void m_grid_MouseClick(object sender, MouseEventArgs e)
    {
      var info = m_grid.HitTest(e.X, e.Y);
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

    private void m_grid_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
    {
      if(!this.Text.EndsWith("*"))
        this.Text += "*";
    }

    private void m_grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || m_filtering)
        return;

      var row = m_grid.Rows[e.RowIndex].DataBoundItem as DataRowView;

      if (row == null)
        return;

      if (row.Row.RowState != DataRowState.Unchanged && !this.Text.EndsWith("*"))
        this.Text += "*";
    }

    private void m_context_menu_Opening(object sender, CancelEventArgs e)
    {
      if (m_grid.ReadOnly)
        e.Cancel = true;
    }

    private void UpdateAutoFilterColumnWidths(int startColumn)
    {
      if (m_auto_resizing)
        return;

      for (int i = startColumn; i < m_filter_row.Count; i++)
      {
        var textBox = m_filter_row[i];

        // Позиционирование поля
        Rectangle headerRect = m_grid.GetCellDisplayRectangle(i, -1, true);
        textBox.Location = new Point(headerRect.X + 3, headerRect.Height - m_grid.ColumnHeadersDefaultCellStyle.Padding.Bottom);
        textBox.Size = new Size(headerRect.Width - 6, headerRect.Height);
      }
    }

    private void m_grid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
    {
      this.UpdateAutoFilterColumnWidths(e.Column.Index);
    }

    private void m_grid_RowHeadersWidthChanged(object sender, EventArgs e)
    {
      this.UpdateAutoFilterColumnWidths(0);
    }

    private void m_grid_Scroll(object sender, ScrollEventArgs e)
    {
      if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
        this.UpdateAutoFilterColumnWidths(0);
    }
  }
}