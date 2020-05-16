using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.Loader
{
  /// <summary>
  /// Интерфейс для типа данных, который будет инициализировать сборку
  /// </summary>
  public interface ILibraryInitializer
  {
    void Perform();
  }

  /// <summary>
  /// Атрибут сборки, указывающий, какой тип данных будет инициализировать сборку
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
  public sealed class LibraryInitializerAttribute : Attribute
  {
    private readonly Type m_startup_type;

    public LibraryInitializerAttribute(Type startupType)
    {
      if (startupType == null)
        throw new ArgumentNullException("startupType");

      if (!typeof(ILibraryInitializer).IsAssignableFrom(startupType))
        throw new ArgumentException();

      m_startup_type = startupType;
    }

    public Type StartupType
    {
      get { return m_startup_type; }
    }

    internal void Perform()
    {
      ((ILibraryInitializer)Activator.CreateInstance(m_startup_type)).Perform();
    }
  }
}
