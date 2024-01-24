using SqlDbImport.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SqlDbImport
{
  public partial class SqlDbImportForm : Form
  {
    public SqlDbImportForm()
    {
      InitializeComponent();

      if(Settings.Default != null)
      {
        m_server.Text = Settings.Default.Server;
        m_database.Text = Settings.Default.Database;
        m_table.Text = Settings.Default.Table;
        m_login.Text = Settings.Default.Login;
        cb_integrated_security.Checked = Settings.Default.IntegratedSecurity;
      }
      m_password.Text = string.Empty;
      m_password.PasswordChar = '*';
    }

    public string ServerPath
    {
      get { return m_server.Text; }
    }

    public string DatabaseName
    {
      get { return m_database.Text; }
    }

    public string TableName
    {
      get { return m_table.Text; }
    }
    public string Login
    {
      get { return m_login.Text; }
    }
    public string Password
    {
      get { return m_password.Text; }
    }
    public bool IntegratedSecurity
    {
      get { return cb_integrated_security.Checked; }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      Settings.Default.Server = m_server.Text;
      Settings.Default.Database = m_database.Text;
      Settings.Default.Table = m_table.Text;
      Settings.Default.Login = m_login.Text;
      Settings.Default.IntegratedSecurity = cb_integrated_security.Checked;
      Settings.Default.Save();

      base.OnFormClosing(e);

      if (this.DialogResult != DialogResult.OK)
        return;

      List<string> messages = new List<string>();

      if (string.IsNullOrEmpty(m_server.Text))
        messages.Add(Resources.NO_SERVER);

      if (string.IsNullOrEmpty(m_database.Text))
        messages.Add(Resources.NO_DATABASE);

      if (string.IsNullOrEmpty(m_table.Text))
        messages.Add(Resources.NO_TABLE);

      if (!cb_integrated_security.Checked)
      {
        if (string.IsNullOrEmpty(m_login.Text))
          messages.Add(Resources.NO_LOGIN);
      }

      if (messages.Count > 0)
      {
        MessageBox.Show(string.Join(Environment.NewLine, messages),
          this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        e.Cancel = true;
      }
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
        m_login.Visible = !cb_integrated_security.Checked;
        m_password.Visible = !cb_integrated_security.Checked;
        lb_login.Visible = !cb_integrated_security.Checked;
        lb_password.Visible = !cb_integrated_security.Checked;
    }
  }
}