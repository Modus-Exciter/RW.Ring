using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using Notung.Data;
using Notung.Threading;

namespace Notung.Net
{
  public interface ICallSerializer
  {
    void Serialize(ISerializationCommand command, Stream stream);

    ISerializationCommand Deserialize(Stream stream);
  }

  public class BinaryCallSerializer : ICallSerializer
  {
    private readonly BinaryFormatter m_formatter = new BinaryFormatter();

    public void Serialize(ISerializationCommand command, Stream stream)
    {
      m_formatter.Serialize(stream, command);
    }

    public ISerializationCommand Deserialize(Stream stream)
    {
      return (ISerializationCommand)m_formatter.Deserialize(stream);
    }
  }

  public class DataContractCallSerializer : ICallSerializer
  {
    private readonly Dictionary<Type, DataContractSerializer> m_seriazlizers = new Dictionary<Type, DataContractSerializer>();
    private readonly SharedLock m_lock = new SharedLock(false);

    private DataContractSerializer GetSerializer(Type type)
    {
      DataContractSerializer ret;

      using (m_lock.ReadLock())
      {
        if (m_seriazlizers.TryGetValue(type, out ret))
          return ret;
      }

      using (m_lock.WriteLock())
      {
        if (!m_seriazlizers.TryGetValue(type, out ret))
        {
          ret = new DataContractSerializer(type);
          m_seriazlizers.Add(type, ret);
        }
      }

      return ret;
    }

    public void Serialize(ISerializationCommand command, Stream stream)
    {
      /* По идее, вместо SerializationCommand должен быть какой-то интерфейс команды.
       * Если это действительно SerializationCommand, то нужно пометить её буквой m
       * и использвать указанный метод сериализации. В противном случае, необходимо
       * в начальный комментарий вписать не метод, а тип данных, а при десериализации
       * просто создавать DataContractSerializer
       */
      var cmd = command as SerializationCommand;
      StreamWriter sw = new StreamWriter(stream);

      if (cmd != null)
        sw.WriteLine("<!--m:{0}-->", cmd.Method);
      else
        sw.WriteLine("<!--{0}, {1}-->", command.GetType().FullName, command.GetType().Assembly.GetName().Name);

      sw.Flush();

      if (cmd != null)
        GetSerializer(cmd.Parameters.GetType()).WriteObject(stream, cmd.Parameters);
      else
        GetSerializer(command.GetType()).WriteObject(stream, command);
    }

    public ISerializationCommand Deserialize(Stream stream)
    {
      StreamReader sr = new StreamReader(stream);
      string header = sr.ReadLine();
      bool is_method_call = false;

      if (header.StartsWith("<!--m:"))
      {
        header = header.Substring("<!--m:".Length, header.Length - "<!--m:".Length - "-->".Length);
        is_method_call = true;
      }
      else if (header.StartsWith("<!--"))
        header = header.Substring("<!--".Length, header.Length - "<!--".Length - "-->".Length);

      if (is_method_call)
      {
        var type = ParametersList.GetRequiredType(SerializationCommand.GetMethod(header));

        return new SerializationCommand(header, 
          (IParametersList)GetSerializer(type).ReadObject(new XmlTextReader(sr)));
      }
      else
      {
        return (ISerializationCommand)GetSerializer(Type.GetType(header)).ReadObject(new XmlTextReader(sr));
      }
    }
  }

  public interface ISerializationCommand
  {
    object Call(object instance);
  }

  [Serializable, DataContract(Namespace = "")]
  public sealed class SerializationCommand : ISerializationCommand
  {
    [DataMember(Name = "Method")]
    private readonly string m_method;
    [DataMember(Name = "Parameters")]
    private readonly IParametersList m_parameters;

    public SerializationCommand(MethodInfo method, object[] parameters)
    {
      Debug.Assert(method != null);

      if (parameters == null)
        parameters = ArrayExtensions.Empty<object>();

      m_method = string.Format("{0}, {1}.{2}", method.DeclaringType.FullName,
        method.DeclaringType.Assembly.GetName().Name, method.Name);
      m_parameters = ParametersList.Create(method, parameters);
    }

    public SerializationCommand(string method, IParametersList parameters)
    {
      Debug.Assert(method != null);
      Debug.Assert(parameters != null);

      m_method = method;
      m_parameters = parameters;
    }

    public string Method
    {
      get { return m_method; }
    }

    public IParametersList Parameters
    {
      get { return m_parameters; }
    }

    public static MethodInfo GetMethod(string method)
    {
      var sep = method.LastIndexOf('.');

      return Type.GetType(method.Substring(0, sep)).GetMethod(method.Substring(sep + 1));
    }

    public object Call(object instance)
    {
      return GetMethod(m_method).Invoke(instance, m_parameters.GetValues());
    }
  }
}
