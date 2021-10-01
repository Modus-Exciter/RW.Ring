using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;

namespace CertGenerator
{
  public static class NetshHepler
  {
    public static readonly IList<CommandResult> Log = new BindingList<CommandResult>();
    
    public static string GetLocalHost()
    {
      var dir = Environment.GetFolderPath(Environment.SpecialFolder.System);
      var file = Path.Combine(dir, "drivers\\etc\\hosts");

      using (var reader = File.OpenText(file))
      {
        while (!reader.EndOfStream)
        {
          var line = reader.ReadLine().Trim();

          if (line.StartsWith("#"))
            continue;

          var end = line.IndexOf('#');

          if (end > 0)
            line = line.Substring(0, end);

          var words = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

          if (words.Length == 2 && IPAddress.TryParse(words[0], out IPAddress address))
          {
            if (IPAddress.IsLoopback(address))
              return words[1];
          }
        }
      }

      return "localhost";
    }

    public static bool HasCertificate(ushort port)
    {
      return RunCommand(string.Format("http show sslcert ipport=0.0.0.0:{0}", port));
    }

    public static bool RemoveCertificate(ushort port)
    {
      return RunCommand(string.Format("http delete sslcert ipport=0.0.0.0:{0}", port));
    }

    public static bool AddCertificate(ushort port, string thumbPrint, string appId)
    {
      return RunCommand(string.Format(
        "http add sslcert ipport=0.0.0.0:{0} certhash={1} certstorename=My appid={2}{3}{4}",
        port, thumbPrint, "{", appId, "}"));
    }

    private static bool RunCommand(string command)
    {
      var startInfo = new ProcessStartInfo("netsh.exe", command);

      startInfo.UseShellExecute = false;
      startInfo.RedirectStandardOutput = true;
      startInfo.CreateNoWindow = true;

      using (var proc = Process.Start(startInfo))
      {
        proc.WaitForExit();

        Log.Add(new CommandResult 
        { 
          Text = proc.StandardOutput.ReadToEnd().Trim(),
          IsError = proc.ExitCode != 0
        });

        return proc.ExitCode == 0;
      }
    }
  }

  public sealed class CommandResult
  {
    public string Text { get; set; }

    public bool IsError { get; set; }

    public override string ToString()
    {
      return this.Text ?? base.ToString();
    }
  }
}
