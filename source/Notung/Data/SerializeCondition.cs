using System;
using System.Runtime.Serialization;

namespace Notung.Data
{
  /// <summary>
  /// Структура, которая позволяет сериализовывать или не сериализовывать поле 
  /// в зависимости от сериализуемости реального типа объекта, хранящегося в нём
  /// </summary>
  /// <typeparam name="T">Тип условно сериализуемого поля</typeparam>
  [Serializable]
  public struct SerializeCondition<T> : ISerializable
  {
    static SerializeCondition()
    {
      if (typeof(T).IsClass && !typeof(T).IsDefined(typeof(SerializableAttribute), false))
        throw new SerializationException(string.Format(
          "Type '{0}' in Assembly '{1}' is not marked as serializable.", typeof(T), typeof(T).Assembly));
    }
    
    public SerializeCondition(T value) : this()
    {
      this.Value = value;
    }

    private SerializeCondition(SerializationInfo info, StreamingContext context) : this()
    {
      this.Value = (T)info.GetValue("Value", typeof(T));
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      T value = this.Value;

      if (!CanSerialize)
        info.AddValue("Value", null);
      else
        info.AddValue("Value", this.Value);
    }

    public override string ToString()
    {
      T value = this.Value;

      if (value == null)
        return "null";
      else
        return value.ToString();
    }

    public override bool Equals(object obj)
    {
      if (!(obj is SerializeCondition<T>))
        return false;

      var other = (SerializeCondition<T>)obj;

      return object.Equals(this.Value, other.Value);
    }

    public override int GetHashCode()
    {
      T value = this.Value;

      if (value == null)
        return typeof(T).GetHashCode();
      else
        return value.GetHashCode();
    }

    public bool CanSerialize
    {
      get { return Value != null && Value.GetType().IsDefined(typeof(SerializableAttribute), false); }
    }

    public T Value { get; set; }
  }
}
