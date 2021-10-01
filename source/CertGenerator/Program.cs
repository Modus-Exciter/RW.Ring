using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace CertGenerator
{
  class Program
  {
    static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.Run(new MainForm(args));
    }

    public static bool CreateAndSetupCertificates(string hostName, ushort port)
    {
      var server = CertificateHelper.Find(hostName, CertificateHelper.MY);

      if (server == null)
      {
        var root = new X509Certificate2(Properties.Resources.Certificate, "terraline");
        var old_root = CertificateHelper.Find(root.Subject.Substring(3), CertificateHelper.ROOT);

        if (old_root != null && old_root.Thumbprint != root.Thumbprint)
        {
          CertificateHelper.Remove(old_root, CertificateHelper.ROOT);
          CertificateHelper.Add(root, CertificateHelper.ROOT, false);
        }
        else if (old_root == null)
          CertificateHelper.Add(root, CertificateHelper.ROOT, false);

        server = CertificateHelper.CreateServerCertificate(root, hostName);
        CertificateHelper.Add(server, CertificateHelper.MY, true);
      }

      if (NetshHepler.HasCertificate(port))
        NetshHepler.RemoveCertificate(port);

      return NetshHepler.AddCertificate(port, server.Thumbprint, "39e1e6db-d351-411a-83e4-b84e1144afad");
    }
  }
}
