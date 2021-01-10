using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace Notung.Net
{
  /// <summary>
  /// Объект для упаковки параметров удалённой операции
  /// </summary>
  public interface IParametersList
  {
    /// <summary>
    /// Получает типы параметров удалённой операции
    /// </summary>
    /// <returns>Типы параметров удалённой операции</returns>
    Type[] GetTypes();

    /// <summary>
    /// Получает значения параметров удалённой операции
    /// </summary>
    /// <returns>Значения параметров удалённой операции</returns>
    object[] GetValues();
  }

  /// <summary>
  /// Работа с параметрами удалённой операции
  /// </summary>
  [Serializable, DataContract(Name = "PL", Namespace = "")]
  public sealed class ParametersList : IParametersList
  {
    private static readonly ParametersList _instance = new ParametersList();

    private ParametersList() { }

    /// <summary>
    /// Преобразование метода и значений его параметров в объект упаковки
    /// </summary>
    /// <param name="method">Метод, для которого нужно сформировать набор параметров</param>
    /// <param name="values">Значения параметров для метода</param>
    /// <returns>Объект для упаковки параметров удалённой операции</returns>
    public static IParametersList Create(MethodInfo method, params object[] values)
    {
      Debug.Assert(method != null);
      Debug.Assert(values != null);

      return (IParametersList)Activator.CreateInstance(GetRequiredType(method, values), values);
    }

    /// <summary>
    /// Получение типа объекта для упаковки, подходящего для вызова удалённого метода
    /// </summary>
    /// <param name="method">метод, который нужно вызвать удалённо</param>
    /// <returns>Тип объекта упаковки</returns>
    public static Type GetRequiredType(MethodInfo method)
    {
      Debug.Assert(method != null);
      return GetRequiredType(method, null);
    }

    private static Type GetRequiredType(MethodInfo method, object[] values)
    {
      var parameters = method.GetParameters();

      if (values != null && values.Length != parameters.Length)
        throw new ArgumentException("Parameters count mismatch");

      if (parameters.Length == 0)
        return typeof(ParametersList);

      var types = new Type[parameters.Length];

      for (int i = 0; i < parameters.Length; i++)
      {
        types[i] = parameters[i].ParameterType.IsByRef ?
          parameters[i].ParameterType.GetElementType() : parameters[i].ParameterType;
      }

      Type baseType;
      switch (types.Length)
      {
        case 1:
          baseType = typeof(ParamList<>);
          break;
        case 2:
          baseType = typeof(ParamList<,>);
          break;
        case 3:
          baseType = typeof(ParamList<,,>);
          break;
        case 4:
          baseType = typeof(ParamList<,,,>);
          break;
        case 5:
          baseType = typeof(ParamList<,,,,>);
          break;
        case 6:
          baseType = typeof(ParamList<,,,,,>);
          break;
        case 7:
          baseType = typeof(ParamList<,,,,,,>);
          break;
        case 8:
          baseType = typeof(ParamList<,,,,,,,>);
          break;
        default:
          throw new ArgumentException("Too many parameters");
      }

      return baseType.MakeGenericType(types);
    }

    Type[] IParametersList.GetTypes()
    {
      return Type.EmptyTypes;
    }

    object[] IParametersList.GetValues()
    {
      return Global.EmptyArgs;
    }

    [Serializable, DataContract]
    private abstract class ParametersListBase : IParametersList
    {
      public abstract Type[] GetTypes();

      public abstract object[] GetValues();

      public sealed override string ToString()
      {
        return string.Join(", ", GetValues());
      }
    }

    [Serializable, DataContract(Name = "PL", Namespace = "")]
    private sealed class ParamList<T> : ParametersListBase
    {
      private static readonly Type[] _types = new Type[] { typeof(T) };

      [DataMember(Name = "P", EmitDefaultValue = false)]
      private readonly T m_1;

      public ParamList(T p1)
      {
        m_1 = p1;
      }

      public override Type[] GetTypes()
      {
        return _types;
      }

      public override object[] GetValues()
      {
        return new object[] { m_1 };
      }
    }

    [Serializable, DataContract(Name = "PL", Namespace = "")]
    private sealed class ParamList<T1, T2> : ParametersListBase
    {
      private static readonly Type[] _types = new Type[] { typeof(T1), typeof(T2) };

      [DataMember(Name = "A", EmitDefaultValue = false)]
      private readonly T1 m_1;
      [DataMember(Name = "B", EmitDefaultValue = false)]
      private readonly T2 m_2;

      public ParamList(T1 p1, T2 p2)
      {
        m_1 = p1;
        m_2 = p2;
      }

      public override Type[] GetTypes()
      {
        return _types;
      }

      public override object[] GetValues()
      {
        return new object[] { m_1, m_2 };
      }
    }

    [Serializable, DataContract(Name = "PL", Namespace = "")]
    private sealed class ParamList<T1, T2, T3> : ParametersListBase
    {
      private static readonly Type[] _types = new Type[] { typeof(T1), typeof(T2), typeof(T3) };

      [DataMember(Name = "A", EmitDefaultValue = false)]
      private readonly T1 m_1;
      [DataMember(Name = "B", EmitDefaultValue = false)]
      private readonly T2 m_2;
      [DataMember(Name = "C", EmitDefaultValue = false)]
      private readonly T3 m_3;

      public ParamList(T1 p1, T2 p2, T3 p3)
      {
        m_1 = p1;
        m_2 = p2;
        m_3 = p3;
      }

      public override Type[] GetTypes()
      {
        return _types;
      }

      public override object[] GetValues()
      {
        return new object[] { m_1, m_2, m_3 };
      }
    }

    [Serializable, DataContract(Name = "PL", Namespace = "")]
    private sealed class ParamList<T1, T2, T3, T4> : ParametersListBase
    {
      private static readonly Type[] _types = new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };

      [DataMember(Name = "A", EmitDefaultValue = false)]
      private readonly T1 m_1;
      [DataMember(Name = "B", EmitDefaultValue = false)]
      private readonly T2 m_2;
      [DataMember(Name = "C", EmitDefaultValue = false)]
      private readonly T3 m_3;
      [DataMember(Name = "D", EmitDefaultValue = false)]
      private readonly T4 m_4;

      public ParamList(T1 p1, T2 p2, T3 p3, T4 p4)
      {
        m_1 = p1;
        m_2 = p2;
        m_3 = p3;
        m_4 = p4;
      }

      public override Type[] GetTypes()
      {
        return _types;
      }

      public override object[] GetValues()
      {
        return new object[] { m_1, m_2, m_3, m_4 };
      }
    }

    [Serializable, DataContract(Name = "PL", Namespace = "")]
    private sealed class ParamList<T1, T2, T3, T4, T5> : ParametersListBase
    {
      private static readonly Type[] _types = new Type[] 
      { 
        typeof(T1),
        typeof(T2),
        typeof(T3),
        typeof(T4),
        typeof(T5)
      };

      [DataMember(Name = "A", EmitDefaultValue = false)]
      private readonly T1 m_1;
      [DataMember(Name = "B", EmitDefaultValue = false)]
      private readonly T2 m_2;
      [DataMember(Name = "C", EmitDefaultValue = false)]
      private readonly T3 m_3;
      [DataMember(Name = "D", EmitDefaultValue = false)]
      private readonly T4 m_4;
      [DataMember(Name = "E", EmitDefaultValue = false)]
      private readonly T5 m_5;

      public ParamList(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
      {
        m_1 = p1;
        m_2 = p2;
        m_3 = p3;
        m_4 = p4;
        m_5 = p5;
      }

      public override Type[] GetTypes()
      {
        return _types;
      }

      public override object[] GetValues()
      {
        return new object[] { m_1, m_2, m_3, m_4, m_5 };
      }
    }

    [Serializable, DataContract(Name = "PL", Namespace = "")]
    private sealed class ParamList<T1, T2, T3, T4, T5, T6> : ParametersListBase
    {
      private static readonly Type[] _types = new Type[] 
      { 
        typeof(T1),
        typeof(T2),
        typeof(T3),
        typeof(T4),
        typeof(T5),
        typeof(T6)
      };

      [DataMember(Name = "A", EmitDefaultValue = false)]
      private readonly T1 m_1;
      [DataMember(Name = "B", EmitDefaultValue = false)]
      private readonly T2 m_2;
      [DataMember(Name = "C", EmitDefaultValue = false)]
      private readonly T3 m_3;
      [DataMember(Name = "D", EmitDefaultValue = false)]
      private readonly T4 m_4;
      [DataMember(Name = "E", EmitDefaultValue = false)]
      private readonly T5 m_5;
      [DataMember(Name = "F", EmitDefaultValue = false)]
      private readonly T6 m_6;

      public ParamList(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
      {
        m_1 = p1;
        m_2 = p2;
        m_3 = p3;
        m_4 = p4;
        m_5 = p5;
        m_6 = p6;
      }

      public override Type[] GetTypes()
      {
        return _types;
      }

      public override object[] GetValues()
      {
        return new object[] { m_1, m_2, m_3, m_4, m_5, m_6 };
      }
    }

    [Serializable, DataContract(Name = "PL", Namespace = "")]
    private sealed class ParamList<T1, T2, T3, T4, T5, T6, T7> : ParametersListBase
    {
      private static readonly Type[] _types = new Type[] 
      { 
        typeof(T1),
        typeof(T2),
        typeof(T3),
        typeof(T4),
        typeof(T5),
        typeof(T6),
        typeof(T7)
      };

      [DataMember(Name = "A", EmitDefaultValue = false)]
      private readonly T1 m_1;
      [DataMember(Name = "B", EmitDefaultValue = false)]
      private readonly T2 m_2;
      [DataMember(Name = "C", EmitDefaultValue = false)]
      private readonly T3 m_3;
      [DataMember(Name = "D", EmitDefaultValue = false)]
      private readonly T4 m_4;
      [DataMember(Name = "E", EmitDefaultValue = false)]
      private readonly T5 m_5;
      [DataMember(Name = "F", EmitDefaultValue = false)]
      private readonly T6 m_6;
      [DataMember(Name = "G", EmitDefaultValue = false)]
      private readonly T7 m_7;

      public ParamList(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
      {
        m_1 = p1;
        m_2 = p2;
        m_3 = p3;
        m_4 = p4;
        m_5 = p5;
        m_6 = p6;
        m_7 = p7;
      }

      public override Type[] GetTypes()
      {
        return _types;
      }

      public override object[] GetValues()
      {
        return new object[] { m_1, m_2, m_3, m_4, m_5, m_6, m_7 };
      }
    }

    [Serializable, DataContract(Name = "PL", Namespace = "")]
    private sealed class ParamList<T1, T2, T3, T4, T5, T6, T7, T8> : ParametersListBase
    {
      private static readonly Type[] _types = new Type[] 
      { 
        typeof(T1),
        typeof(T2),
        typeof(T3),
        typeof(T4),
        typeof(T5),
        typeof(T6),
        typeof(T7),
        typeof(T8)
      };

      [DataMember(Name = "A", EmitDefaultValue = false)]
      private readonly T1 m_1;
      [DataMember(Name = "B", EmitDefaultValue = false)]
      private readonly T2 m_2;
      [DataMember(Name = "C", EmitDefaultValue = false)]
      private readonly T3 m_3;
      [DataMember(Name = "D", EmitDefaultValue = false)]
      private readonly T4 m_4;
      [DataMember(Name = "E", EmitDefaultValue = false)]
      private readonly T5 m_5;
      [DataMember(Name = "F", EmitDefaultValue = false)]
      private readonly T6 m_6;
      [DataMember(Name = "G", EmitDefaultValue = false)]
      private readonly T7 m_7;
      [DataMember(Name = "H", EmitDefaultValue = false)]
      private readonly T8 m_8;

      public ParamList(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
      {
        m_1 = p1;
        m_2 = p2;
        m_3 = p3;
        m_4 = p4;
        m_5 = p5;
        m_6 = p6;
        m_7 = p7;
        m_8 = p8;
      }

      public override Type[] GetTypes()
      {
        return _types;
      }

      public override object[] GetValues()
      {
        return new object[] { m_1, m_2, m_3, m_4, m_5, m_6, m_7, m_8 };
      }
    }
  }
}