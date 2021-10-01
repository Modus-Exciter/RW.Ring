using System.Collections.Generic;

namespace CertGenerator
{
  public static class CommandLineHelper
  {
    public static CommandLineArgs ParseArgs(string[] args)
    {
      var ret = new CommandLineArgs();

      if (args == null)
        return ret;

      string option = null;

      foreach (var arg in args)
      {
        if (arg.StartsWith("-"))
        {
          if (option == null)
            option = arg;
          else
          {
            ret.Options.Add(option, null);
            option = arg;
          }
        }
        else if (option != null)
        {
          ret.Options.Add(option, arg);
          option = null;
        }
        else
          ret.Parameters.Add(arg);
      }

      if (option != null)
        ret.Options.Add(option, null);

      return ret;
    }

    public static string GetHost(CommandLineArgs cmd)
    {
      if (cmd.Options.ContainsKey("-host"))
        return cmd.Options["-host"];
      else if (cmd.Parameters.Count > 0)
        return cmd.Parameters[0];
      else
        return NetshHepler.GetLocalHost();
    }

    public static ushort GetPort(CommandLineArgs cmd)
    {
      if (cmd.Options.ContainsKey("-port"))
        return ushort.Parse(cmd.Options["-port"]);
      else if (cmd.Options.ContainsKey("-host") && cmd.Parameters.Count > 0)
        return ushort.Parse(cmd.Parameters[0]);
      else if (!cmd.Options.ContainsKey("-host") && cmd.Parameters.Count > 1)
        return ushort.Parse(cmd.Parameters[1]);
      else
        return 8000;
    }
  }

  public class CommandLineArgs
  {
    private readonly Dictionary<string, string> m_options = new Dictionary<string, string>();
    private readonly List<string> m_parameters = new List<string>();

    public Dictionary<string, string> Options
    {
      get { return m_options; }
    }

    public List<string> Parameters
    {
      get { return m_parameters; }
    }
  }
}
