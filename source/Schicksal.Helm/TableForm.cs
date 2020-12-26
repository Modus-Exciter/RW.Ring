using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Notung;
using Schicksal.Helm.Properties;

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
        var bf = new BinaryFormatter();
        bf.Serialize(fs, graph);
      }
    }

    public void Save()
    {
      if (string.IsNullOrEmpty(this.FileName))
        this.SaveAs();
      else
        this.SaveFile(this.FileName, this.DataSource);

      this.Changed = false;
      this.FileName = Path.GetFileName(this.FileName);
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
          AppManager.Configurator.GetSection<Program.Preferences>().LastFiles.Add(dlg.FileName);
          this.Changed = false;
          this.FileName = Path.GetFileName(this.FileName);
        }
      }
    }
  }
}
