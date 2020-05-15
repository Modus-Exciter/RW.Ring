using System;
using System.IO;
using System.Reflection;

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

    public Assembly Assembly { get; internal set; }

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
