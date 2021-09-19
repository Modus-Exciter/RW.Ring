using System;
using System.IO;
using Notung.Helm.Windows.Properties;
using Notung.Services;

namespace Notung.Helm.Windows
{
  /// <summary>
  /// Фоновая задача, запускающая функцию без параметров и возвращаемого значения из нативной dll.
  /// </summary>
  public class ExternalCallWork : RunBase
  {
    private readonly Action m_function;
    private readonly string m_file_name;
    private Exception m_exception;

    public ExternalCallWork(NativeDll dll, string function)
    {
      if (dll == null)
        throw new ArgumentNullException("dll");

      if (string.IsNullOrWhiteSpace(function))
        throw new ArgumentException(Resources.DLL_NO_FUNCTION);

      m_file_name = dll.Path;
      m_function = dll.GetFunction<Action>(function);

      if (m_function == null)
        throw new MissingMethodException();
    }

    public bool OnlySTAThread { get; set; }

    public override void Run()
    {
      string last_dir = Environment.CurrentDirectory;
      Environment.CurrentDirectory = Path.GetDirectoryName(m_file_name);

      try
      {
        if (this.OnlySTAThread)
          AppManager.Instance.ApartmentWrapper.Invoke(this.RunFunction);
        else
          m_function();

        if (m_exception != null)
          throw m_exception;
      }
      finally
      {
        Environment.CurrentDirectory = last_dir;
      }
    }

    private void RunFunction()
    {
      try
      {
        m_function();
      }
      catch (Exception ex)
      {
        m_exception = ex;
      }
    }
  }
}
