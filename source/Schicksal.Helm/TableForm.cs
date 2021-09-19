using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Notung;
using Notung.Services;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm
{
  public partial class TableForm : Form
  {
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
          value.AcceptChanges();
      }
    }

    public string FileName { get; set; }

    public void MarkAsReadOnly()
    {
      m_grid.ReadOnly = true;
      m_grid.AllowUserToAddRows = false;
      m_grid.AllowUserToDeleteRows = false;

      m_cmd_remove.Visible = false;

      foreach (DataGridViewColumn col in m_grid.Columns)
      {
        if (col.ValueType == typeof(double) || col.ValueType == typeof(float))
          col.DefaultCellStyle.Format = "0.000";
      }
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
        m_grid.AutoResizeColumns();
    }

    private void Grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;

      var row = m_grid.Rows[e.RowIndex].DataBoundItem as DataRowView;

      if (row == null)
        return;

      if (row.Row.RowState != DataRowState.Unchanged && !this.Text.EndsWith("*"))
        this.Text += "*";
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

    private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      AppManager.Notificator.Show(e.Exception.Message, InfoLevel.Error);
    }

    private void Switcher_LanguageChanged(object sender, Notung.ComponentModel.LanguageEventArgs e)
    {
      m_cmd_remove.Text = Resources.REMOVE;
    }

    private void Cmd_remove_Click(object sender, EventArgs e)
    {
      if (m_grid.Rows.Count > 0 && m_grid.AllowUserToDeleteRows && m_grid.SelectedRows.Count > 0)
        m_grid.Rows.Remove(m_grid.SelectedRows[0]);
    }
  }
}