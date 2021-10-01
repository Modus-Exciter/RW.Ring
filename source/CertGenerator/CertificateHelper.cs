using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertGenerator
{
  public static class CertificateHelper
  {
    public const string ROOT = "Root";
    public const string MY = "My";
    
    public static X509Certificate2 Find(string name, string location, bool withPrivateKey = true)
    {
      using (var store = new X509Store(location, StoreLocation.LocalMachine))
      {
        store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
        var result = store.Certificates.Find(X509FindType.FindBySubjectName, name, false);

        if (result.Count == 0)
          return null;
        else if (result.Count == 1)
          return result[0];
        else
          return result.Cast<X509Certificate2>().FirstOrDefault(c => c.HasPrivateKey == withPrivateKey);
      }
    }

    public static void Add(X509Certificate2 certificate, string location, bool withPrivateKey)
    {
      using (var store = new X509Store(location, StoreLocation.LocalMachine))
      {
        store.Open(OpenFlags.ReadWrite);

        if (withPrivateKey && certificate.HasPrivateKey)
        {
          var password = GetLongTestString(new Random().Next(10, 21));

          using (var copy = new X509Certificate2(
            certificate.Export(X509ContentType.Pfx, password), password,
            X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet))
          {
            store.Add(copy);
          }
        }
        else
          store.Add(certificate);
      }
    }

    public static bool Remove(X509Certificate2 certificate, string location)
    {
      if (certificate == null)
        throw new ArgumentNullException("certificate");

      using (var store = new X509Store(location, StoreLocation.LocalMachine))
      {
        store.Open(OpenFlags.ReadWrite);
        var old = store.Certificates.Count;
        store.Remove(certificate);
        return old > store.Certificates.Count;
      }
    }

    public static X509Certificate2 CreateRootCertificate(string orgatizationName, string fullOrganizationName = null)
    {
      using (var rsaKey = RSA.Create(4096))
      {
        var request = new CertificateRequest(
           string.Format("CN={0}", orgatizationName),
           rsaKey,
           HashAlgorithmName.SHA256,
           RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature |
          X509KeyUsageFlags.KeyCertSign, true));

        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));

        request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

        var ret = request.CreateSelfSigned(
                  DateTimeOffset.UtcNow.AddDays(-1),
                  DateTimeOffset.UtcNow.AddYears(10));

        ret.FriendlyName = fullOrganizationName ??  orgatizationName;

        return ret;
      }
    }

    public static X509Certificate2 CreateServerCertificate(X509Certificate2 parentCertificate, string hostName)
    {
      if (parentCertificate == null)
        throw new ArgumentNullException("parentCertificate");

      using (var rsaKey = RSA.Create(2048))
      {
        SubjectAlternativeNameBuilder subjectAlternativeNameBuilder = CreateAlternativeName(hostName);

        var request = new CertificateRequest(
            string.Format("CN={0}", hostName),
            rsaKey,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(false, false, 0, false));

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation,
                false));

        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(
                new OidCollection
                {
                  new Oid("1.3.6.1.5.5.7.3.1")
                },
                true));

        request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));
        request.CertificateExtensions.Add(subjectAlternativeNameBuilder.Build(false));

        var notBefore = DateTimeOffset.UtcNow.AddDays(-1);
        var notAfter = DateTimeOffset.UtcNow.AddYears(10);

        if (notBefore < parentCertificate.NotBefore)
          notBefore = parentCertificate.NotBefore;

        if (notAfter > parentCertificate.NotAfter)
          notAfter = parentCertificate.NotAfter;

        var ret = request.Create(
               parentCertificate,
               notBefore,
               notAfter,
               BitConverter.GetBytes(DateTime.Now.ToBinary())).CopyWithPrivateKey(rsaKey);

        ret.FriendlyName = hostName;

        return ret;
      }
    }

    private static SubjectAlternativeNameBuilder CreateAlternativeName(string hostName)
    {
      var subjectAlternativeNameBuilder = new SubjectAlternativeNameBuilder();

      if (IPAddress.TryParse(hostName, out IPAddress address))
        subjectAlternativeNameBuilder.AddIpAddress(address);
      else
        subjectAlternativeNameBuilder.AddDnsName(hostName);

      return subjectAlternativeNameBuilder;
    }

    private static string GetLongTestString(int size)
    {
      char[] chars = "abcdefghijklmnoprstuvwxyz12345678990_".ToCharArray();
      var rnd = new Random();
      return new string((from b in new byte[size] select chars[rnd.Next(chars.Length)]).ToArray<char>());
    }
  }
}
