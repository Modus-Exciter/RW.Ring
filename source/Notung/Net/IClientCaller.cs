using System;
using System.IO;
using System.Runtime.Serialization;

namespace Notung.Net
{
  /// <summary>
  /// Набор операций, поддерживаемых сетевым клиентом
  /// </summary>
  public interface IClientCaller
  {
    /// <summary>
    /// Выполнение удалённой процедуры
    /// </summary>
    /// <param name="serverOperation">Полное логическое имя удалённой процедуры</param>
    /// <param name="request">Сериализуемые параметры удалённой процедуры</param>
    /// <param name="operation">Сведения об удалённой процедуре</param>
    /// <returns>Результат выполнения удалённой процедуры</returns>
    ICallResult Call(string serverOperation, IParametersList request, RpcOperationInfo operation);

    /// <summary>
    /// Обмен с сервером потоками данных
    /// </summary>
    /// <param name="processRequest">Обработчик потока запроса</param>
    /// <param name="processResponse">Обработчик потока ответа</param>
    void StreamExchange(string command, Action<Stream> processRequest, Action<Stream> processResponse);

    /// <summary>
    /// Обмен с сервером массивами данных
    /// </summary>
    /// <param name="data">Данные, отправленные на сервер</param>
    /// <returns>Данные, полученные от сервера</returns>
    byte[] BinaryExchange(string command, byte[] data);
  }

  /// <summary>
  /// Результат выполнения удалённой процедуры
  /// </summary>
  public interface ICallResult
  {
    /// <summary>
    /// Возвращаемое значение удалённой процедуры
    /// </summary>
    object Value { get; set; }

    /// <summary>
    /// Ошибка, возникшая при выполнении процедуры на сервере
    /// </summary>
    ClientServerException Error { get; set; }
  }

  /// <summary>
  /// Ошибка выполнения удалённой процедуры на сервере
  /// </summary>
  [Serializable]
  public sealed class ClientServerException : ApplicationException
  {
    private readonly string m_server_stack;

    private ClientServerException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      m_server_stack = info.GetString("ServerStack");
    }

    public ClientServerException(string message, string stack) : base(message)
    {
      m_server_stack = stack;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ServerStack", m_server_stack);
    }

    public string ServerStack
    {
      get { return m_server_stack; }
    }
  }

  [Serializable, DataContract(Name = "RES", Namespace = "")]
  internal class CallResult<TResult> : ICallResult
  {
    [DataMember(Name = "data")]
    private TResult m_result;
    [DataMember(Name = "error")]
    private ClientServerException m_error;

    public object Value
    {
      get { return m_result; }
      set { m_result = (TResult)value; }
    }

    public ClientServerException Error
    {
      get { return m_error; }
      set { m_error = value; }
    }
  }
}
