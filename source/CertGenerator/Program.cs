using System;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace CertGenerator
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.Run(new MainForm(args));
    }

    public static bool SetupCertificate(string fileName, string password, string hostName, ushort port)
    {
      try
      {
        var cert = new X509Certificate2(fileName, password, 
          X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

        if (!cert.HasPrivateKey)
        {
          NetshHepler.Log.Add(
            new CommandResult
            {
              Text = "В сертификате отсутствует закрытый ключ",
              IsError = true
            });
          return false;
        }

        if (!cert.Subject.Contains("=" + hostName))
        {
          NetshHepler.Log.Add(
           new CommandResult
           {
             Text = "Сертификат предназначен для другого хоста",
             IsError = true
           });
          return false;
        }

        var old_cert = CertificateHelper.Find(hostName, CertificateHelper.MY);

        if (old_cert == null || old_cert.Thumbprint != cert.Thumbprint)
          CertificateHelper.Add(cert, CertificateHelper.MY, true);

        if (NetshHepler.HasCertificate(port))
          NetshHepler.RemoveCertificate(port);

        return NetshHepler.AddCertificate(port, cert.Thumbprint, "39e1e6db-d351-411a-83e4-b84e1144afad");
      }
      catch (Exception ex)
      {
        NetshHepler.Log.Add(
          new CommandResult
          {
            Text = ex.Message,
            IsError = true
          });
        return false;
      }
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
