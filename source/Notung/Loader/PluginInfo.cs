using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Notung.Properties;

namespace Notung.Loader
{
  public class PluginInfo
  {
    private readonly string m_name;
    private readonly string m_asm_file;
    
    public PluginInfo(string name, string assemblyFile)
    {
      if (string.IsNullOrEmpty(assemblyFile))
        throw new ArgumentNullException("assemblyFile");

      if (string.IsNullOrEmpty(name))
        name = Path.GetFileNameWithoutExtension(assemblyFile);

      m_name = name;
      m_asm_file = assemblyFile;
    }

    public string Name
    {
      get { return m_name; }
    }

    public string AssemblyFile
    {
      get { return m_asm_file; }
    }

    public AppDomain Domain { get; internal set; }

    public AssemblyName AssemblyName { get; internal set; }

    public string SearchPattern { get; internal set; }

    internal IList<PluginInfo> Container { get; set; }

    public void Unload()
    {
      if (this.Domain == null)
        return;

      if (this.Container == null)
        throw new InvalidOperationException();

      if (this.Domain == AppDomain.CurrentDomain)
        throw new InvalidOperationException(Resources.UNLOADING_PLUGIN_CURRENT_DOMAIN);

      if (!this.Container.Any(p => p.Domain == this.Domain))
        AppDomain.Unload(this.Domain);

      this.Domain = null;
      this.Container.Remove(this);
    }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(m_name))
        return this.Name;

      else if (!string.IsNullOrEmpty(m_asm_file))
        return this.AssemblyFile;

      return base.ToString();
    }
  }
}
