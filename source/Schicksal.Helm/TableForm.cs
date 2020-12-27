using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Notung;
using Schicksal.Helm.Properties;
using System;

namespace Schicksal.Helm
{
  public partial class TableForm : Form
  {
    public TableForm()
    {
      InitializeComponent();
    }

    public object DataSource
    {
      get { return m_grid.DataSource; }
      set
      {
        m_grid.DataSource = value;

        this.Changed = false;
      }
    }

    public string FileName { get; set; }

    public bool Changed { get; set; }

    public void MarkAsReadOnly()
    {
      m_grid.ReadOnly = true;
      m_grid.AllowUserToAddRows = false;
      m_grid.AllowUserToDeleteRows = false;

      foreach (DataGridViewColumn col in m_grid.Columns)
      {
        if (col.ValueType == typeof(double) || col.ValueType == typeof(float))
          col.DefaultCellStyle.Format = "0.000";
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.Icon = this.Icon.Clone() as System.Drawing.Icon;
    }

    protected override void OnShown(System.EventArgs e)
    {
      base.OnShown(e);

      if (m_grid.Rows.Count < (1 << 10))
        m_grid.AutoResizeColumns();
    }

    private void SetChanged()
    {
      if (!this.Changed)
        this.Text += "*";

      this.Changed = true;
    }

    private void m_grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      SetChanged();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      if (this.Changed)
      {
        var res = AppManager.Notificator.ConfirmOrCancel(string.Format(
          Resources.SAVE_FILE_CONFIRM, this.Text.Substring(0, this.Text.Length - 1)), InfoLevel.Info);

        if (res == null)
          e.Cancel = true;
        else if (res == true)
          this.Save();
      }
    }

    private void SaveFile(string fileName, object graph)
    {
      if (File.Exists(fileName))
        File.Delete(fileName);

      using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      {
        DataTableSaver.WriteDataTable(graph as DataTable, fs);
      }
      AppManager.Configurator.GetSection<Program.Preferences>().LastFiles[fileName] = DateTime.Now;
      this.Changed = false;
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

    private void m_grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      AppManager.Notificator.Show(e.Exception.Message, InfoLevel.Error);
    }
  }
}
