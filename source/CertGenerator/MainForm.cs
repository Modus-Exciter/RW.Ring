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
      if (Program.CreateAndSetupCertificates(m_host_edit.Text, (ushort)m_port_edit.Value))
      {
        MessageBox.Show("Создание сертификата успешно", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Close();
      }
      else
        MessageBox.Show(NetshHepler.Log.Last().Text, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }
}
