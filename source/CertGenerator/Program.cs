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

    //private static void RunConsole()
    //{
    //  var orgName = "ARI";
    //  var hostName = "mail.agrophys.ru";
    //  ushort port = 14488;

    //  var server = CertificateHelper.Find(hostName, CertificateHelper.MY);

    //  if (server == null)
    //  {
    //    var root = new X509Certificate2(Properties.Resources.Certificate, "terraline");
    //    var old_root = CertificateHelper.Find(orgName, CertificateHelper.ROOT);

    //    if (old_root != null && old_root.Thumbprint != root.Thumbprint)
    //    {
    //      CertificateHelper.Remove(old_root, CertificateHelper.ROOT);
    //      CertificateHelper.Add(root, CertificateHelper.ROOT, false);
    //    }

    //    server = CertificateHelper.CreateServerCertificate(root, hostName);
    //    CertificateHelper.Add(server, CertificateHelper.MY, true);
    //  }

    //  if (NetshHepler.HasCertificate(port))
    //    NetshHepler.RemoveCertificate(port);

    //  Console.WriteLine(NetshHepler.AddCertificate(port, server.Thumbprint, "39e1e6db-d351-411a-83e4-b84e1144afad"));

    //  Console.ReadKey();
    //}
  }
}
