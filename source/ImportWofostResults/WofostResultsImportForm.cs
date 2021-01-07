using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImportWofostResults
{
  public partial class WofostResultsImportForm : Form
  {
    public WofostResultsImportForm()
    {
      InitializeComponent();
    }

    public string ExcelFileName
    {
      get { return m_excel_edit.Text; }
    }

    public string ThirdLevelFileName
    {
      get { return m_third_edit.Text; }
    }

    public string SecondLevelFileName
    {
      get { return m_second_edit.Text; }
    }

    private void m_button_open_excel_Click(object sender, EventArgs e)
    {
      this.SelectFile(m_excel_edit, "Excel files|*.xls");
    }

    private void m_button_open_third_Click(object sender, EventArgs e)
    {
      this.SelectFile(m_third_edit, "JSON files|*.json");
    }

    private void m_button_open_second_Click(object sender, EventArgs e)
    {
      this.SelectFile(m_second_edit, "JSON files|*.json");
    }

    private void SelectFile(TextBox textBox, string filter)
    {
      using (var dlg = new OpenFileDialog())
      {
        dlg.Filter = filter;

        if (dlg.ShowDialog(this) == DialogResult.OK)
          textBox.Text = dlg.FileName;
      }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      base.OnFormClosing(e);

      if (this.DialogResult != DialogResult.OK)
        return;

      List<string> messages = new List<string>();

      if (string.IsNullOrEmpty(m_excel_edit.Text))
        messages.Add("Не выбран файл Excel");
      if (string.IsNullOrEmpty(m_third_edit.Text))
        messages.Add("Не выбран файл третьего уровня продуктивности");
      if (string.IsNullOrEmpty(m_second_edit.Text))
        messages.Add("Не выбран файл второго уровня продуктивности");

      if (messages.Count > 0)
      {
        MessageBox.Show(string.Join(Environment.NewLine, messages));
        e.Cancel = true;
      }
    }
  }
}
