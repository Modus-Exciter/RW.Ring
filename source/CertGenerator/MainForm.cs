using System;
using System.Linq;
using System.Windows.Forms;

namespace CertGenerator
{
  public partial class MainForm : Form
  {
    public MainForm(string[] args)
    {
      this.InitializeComponent();

      m_port_edit.Maximum = ushort.MaxValue;

      var cmd = CommandLineHelper.ParseArgs(args);

      m_host_edit.Text = CommandLineHelper.GetHost(cmd);
      m_port_edit.Value = CommandLineHelper.GetPort(cmd);
    }

    private void m_button_run_Click(object sender, EventArgs e)
    {
      if (m_button_run.Text == createToolStripMenuItem.Text)
        this.CreateNewCertificate();
      else
        this.UploadCertificate();
    }

    private void UploadCertificate()
    {
      using (var dialog = new OpenFileDialog())
      {
        dialog.Filter = "Certificate files with private key|*.pfx";

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
          using (var pass = new PasswordForm())
          {
            if (pass.ShowDialog(this) == DialogResult.OK &&
              Program.SetupCertificate(dialog.FileName, pass.Password,
              m_host_edit.Text, (ushort)m_port_edit.Value))
            {
              MessageBox.Show("Установка сертификата успешна", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
              this.Close();
            }
            else
              MessageBox.Show(NetshHepler.Log.Last().Text, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
      }
    }

    private void CreateNewCertificate()
    {
      if (Program.CreateAndSetupCertificates(m_host_edit.Text, (ushort)m_port_edit.Value))
      {
        MessageBox.Show("Создание сертификата успешно", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Close();
      }
      else
        MessageBox.Show(NetshHepler.Log.Last().Text, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void HandleButtonSelected(object sender, EventArgs e)
    {
      m_button_run.Text = ((ToolStripItem)sender).Text;
      m_button_run.Image = ((ToolStripItem)sender).Image;

      if (sender == uploadToolStripMenuItem)
        this.UploadCertificate();
    }
  }
}
