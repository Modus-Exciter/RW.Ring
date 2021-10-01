using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace CertGenerator
{
  public partial class MainForm : Form
  {
    public MainForm(string[] args)
    {
      this.InitializeComponent();

      m_port_edit.Maximum = ushort.MaxValue;

      if (args != null && args.Length > 0)
        m_host_edit.Text = args[0];
      else
        m_host_edit.Text = "localhost";

      ushort port;
      if (args != null && args.Length > 1 && ushort.TryParse(args[1], out port))
        m_port_edit.Value = port;
      else
        m_port_edit.Value = 8000;
    }

    private void m_button_run_Click(object sender, EventArgs e)
    {
      var server = CertificateHelper.Find(m_host_edit.Text, CertificateHelper.MY);

      if (server == null)
      {
        var root = new X509Certificate2(Properties.Resources.Certificate, "terraline");
        var old_root = CertificateHelper.Find(root.Subject.Substring(3), CertificateHelper.ROOT);

        if (old_root != null && old_root.Thumbprint != root.Thumbprint)
        {
          CertificateHelper.Remove(old_root, CertificateHelper.ROOT);
          CertificateHelper.Add(root, CertificateHelper.ROOT, false);
        }

        server = CertificateHelper.CreateServerCertificate(root, m_host_edit.Text);
        CertificateHelper.Add(server, CertificateHelper.MY, true);
      }

      var port = (ushort)m_port_edit.Value;

      if (NetshHepler.HasCertificate(port))
        NetshHepler.RemoveCertificate(port);

      if (NetshHepler.AddCertificate(port, server.Thumbprint, "39e1e6db-d351-411a-83e4-b84e1144afad"))
      {
        MessageBox.Show("Создание сертификата успешно", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Close();
      }
      else
        MessageBox.Show(NetshHepler.Log.Last().Text, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }
}
