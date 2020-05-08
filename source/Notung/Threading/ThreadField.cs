using System;
using System.Threading;

namespace Notung.Threading
{
  /// <summary>
  /// Обёртка над значением, специфичным для каждого потока
  /// </summary>
  /// <typeparam name="TType">Тип значения</typeparam>
  public sealed class ThreadField<TType>
  {
    private readonly LocalDataStoreSlot m_slot = Thread.AllocateDataSlot();

    /// <summary>
    /// Инициализирует обёртку со значением по умолчанию
    /// </summary>
    public ThreadField() { }

    /// <summary>
    /// Инициализирует обёртку на основе переданного значения
    /// </summary>
    /// <param name="value"></param>
    public ThreadField(TType value)
    {
      Thread.SetData(m_slot, value);
    }

    /// <summary>
    /// Значение, заданное для текущего потока
    /// </summary>
    public TType Instance
    {
      get
      {
        var value = Thread.GetData(m_slot);

        if (value is TType)
          return (TType)value;
        else
          return default(TType);
      }
      set
      {
        Thread.SetData(m_slot, value);
      }
    }

    public override string ToString()
    {
      var value = this.Instance;

      if (value == null)
        return CoreResources.NULL;

      return value.ToString();
    }
  }
}
