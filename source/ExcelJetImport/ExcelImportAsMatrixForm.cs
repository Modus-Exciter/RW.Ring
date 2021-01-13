using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JetExcelOleDbImport
{
  internal partial class ExcelImportAsMatrixForm : Form
  {
    public ExcelImportAsMatrixForm()
    {
      InitializeComponent();
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
        messages.Add("Не выбран файл Excel");
      if (string.IsNullOrEmpty(this.TableName))
        messages.Add("Не выбрана таблица");
      if (string.IsNullOrEmpty(this.ColumnName))
        messages.Add("Не задано имя для столбца");
      if (string.IsNullOrEmpty(this.RowName))
        messages.Add("Не задано имя для строки");
      if (string.IsNullOrEmpty(this.CellName))
        messages.Add("Не задано имя для ячейки");

      Utils.CheckFileIsOpen(m_path_edit.Text, messages);

      if (messages.Count > 0)
      {
        MessageBox.Show(string.Join(Environment.NewLine, messages), this.Text);
        e.Cancel = true;
      }
    }

    private void m_button_open_excel_Click(object sender, EventArgs e)
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
