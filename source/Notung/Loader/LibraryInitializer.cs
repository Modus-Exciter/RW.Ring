using System;
using System.Runtime.CompilerServices;
using Notung.Logging;

namespace Notung.Loader
{
  /// <summary>
  /// Атрибут сборки, указывающий, какой тип данных будет инициализировать сборку
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
  public sealed class LibraryInitializerAttribute : Attribute
  {
    private readonly Type m_startup_type;
    private static readonly ILog _log = LogManager.GetLogger(typeof(LibraryInitializerAttribute));

    public LibraryInitializerAttribute(Type startupType)
    {
      if (startupType == null)
        throw new ArgumentNullException("startupType");

      m_startup_type = startupType;
    }

    public Type StartupType
    {
      get { return m_startup_type; }
    }

    internal void Perform()
    {
      try
      {
        RuntimeHelpers.RunClassConstructor(m_startup_type.TypeHandle);
      }
      catch (TypeInitializationException ex)
      {
        _log.Error("Perform(): failed to initialize assembly", ex.InnerException);
      }
    }
  }
}
