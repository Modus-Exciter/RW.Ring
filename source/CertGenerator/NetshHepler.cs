using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace CertGenerator
{
  public static class NetshHepler
  {
    public static readonly IList<CommandResult> Log = new BindingList<CommandResult>();
    
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

      Console.WriteLine("netsh {0}", command);

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
