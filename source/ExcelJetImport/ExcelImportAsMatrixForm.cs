using System;
using System.Collections.Generic;
using System.Windows.Forms;
using JetExcelOleDbImport.Properties;

namespace JetExcelOleDbImport
{
  internal partial class ExcelImportAsMatrixForm : Form
  {
    public ExcelImportAsMatrixForm()
    {
      this.InitializeComponent();
    }

    public string ExcelFileName
    {
      get { return m_path_edit.Text; }
    }

    public string TableName
    {
      get { return m_table_edit.SelectedItem as string; }
    }

    public string RowName
    {
      get { return m_row_edit.Text; }
    }

    public string ColumnName
    {
      get { return m_column_edit.Text; }
    }

    public string CellName
    {
      get { return m_cell_edit.Text; }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      base.OnFormClosing(e);

      if (this.DialogResult != DialogResult.OK)
        return;

      List<string> messages = new List<string>();

      if (string.IsNullOrEmpty(m_path_edit.Text))
        messages.Add(Resources.NO_EXCEL_FILE);

      if (string.IsNullOrEmpty(this.TableName))
        messages.Add(Resources.NO_TABLE);

      if (string.IsNullOrEmpty(this.ColumnName))
        messages.Add(Resources.NO_COLUMN);

      if (string.IsNullOrEmpty(this.RowName))
        messages.Add(Resources.NO_ROW);

      if (string.IsNullOrEmpty(this.CellName))
        messages.Add(Resources.NO_CELL);

      Utils.CheckFileIsOpen(m_path_edit.Text, messages);

      if (messages.Count > 0)
      {
        MessageBox.Show(string.Join(Environment.NewLine, messages), this.Text);
        e.Cancel = true;
      }
    }

    private void Button_open_excel_Click(object sender, EventArgs e)
    {
      using (var dlg = new OpenFileDialog())
      {
        dlg.Filter = "Excel files|*.xls";

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
          m_path_edit.Text = dlg.FileName;
          Utils.FillTableList(m_path_edit.Text, m_table_edit.Items);
        }
      }
    }
  }
}
