using System;
using System.ComponentModel;

namespace Notung.ComponentModel
{
  /// <summary>
  /// Локализуемое описание типа данных
  /// </summary>
  public sealed class DescriptionResAttribute : DescriptionAttribute
  {
    /// <summary>
    /// Инициализирует новую метку
    /// </summary>
    /// <param name="resourceName">Имя ресурса</param>
    /// <param name="targetType">Тип, к которому применён атрибут, находящийся в той же сборке, что и ресурс</param>
    public DescriptionResAttribute(string resourceName, Type targetType) : base(resourceName)
    {
      if (targetType == null)
        throw new ArgumentNullException("targetType");

      this.TargetType = targetType;
    }

    /// <summary>
    /// Тип, к которому применён атрибут
    /// </summary>
    public Type TargetType { get; private set; }

    /// <summary>
    /// Если ресурс найден, возвращает локализованный ресурс. Иначе, возвращает имя ресурса
    /// </summary>
    public override string Description
    {
      get
      {
        try
        {
          string resource = ResourceHelper.GetString(this.TargetType, base.DescriptionValue);

          if (!string.IsNullOrEmpty(resource))
            return resource;
        }
        catch { }

        return base.DescriptionValue;
      }
    }

    /// <summary>
    /// Этот атрибут не может быть атрибутом по умолчанию
    /// </summary>
    /// <returns>False</returns>
    public override bool IsDefaultAttribute()
    {
      return false;
    }

    /// <summary>
    /// Сравнивает атрибут с переданным объектом
    /// </summary>
    /// <param name="obj">Сравниваемый объект</param>
    /// <returns>True, если переданный объект является таким же</returns>
    public override bool Equals(object obj)
    {
      DescriptionResAttribute res = obj as DescriptionResAttribute;

      if (res == this)
        return true;

      if (res != null)
        return (res.DescriptionValue == this.DescriptionValue) && (res.TargetType == this.TargetType);

      return false;
    }

    /// <summary>
    /// Возвращает числовой идентификатор
    /// </summary>
    /// <returns>Числовой идентификатор из имени ресурса и типа, к которому применён атрибут</returns>
    public override int GetHashCode()
    {
      return this.DescriptionValue.GetHashCode() ^ this.TargetType.GetHashCode();
    }
  }
}