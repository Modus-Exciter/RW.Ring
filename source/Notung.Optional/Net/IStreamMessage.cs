using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Notung.Loader;

namespace Notung.Net
{
  public interface IStreamMessage : IDisposable
  {    
    /// <summary>
    /// Поток для чтения или записи данных запроса к серверу
    /// </summary>
    Stream RequestStream { get; }

    /// <summary>
    /// Поток для чтения или записи ответа от сервера
    /// </summary>
    Stream ResponseStream { get; }
  }

  public interface IClientMessage : IStreamMessage
  {
    /// <summary>
    /// Подготовка запроса к серверу
    /// </summary>
    /// <param name="format">Формат данных, в которых будет сохранён запрос</param>
    void PrepareRequest(SerializationFormat format, object item);

    /// <summary>
    /// Завершение запроса к серверу
    /// </summary>
    void EndRequest();
  }

  public interface IServerMessage : IStreamMessage
  {
    /// <summary>
    /// Подготовка ответа от сервера
    /// </summary>
    /// <param name="format">Формат данных, в которых будет сохранён ответ</param>
    void PrepareResponse(SerializationFormat format, object item);

    /// <summary>
    /// Завершение ответа от сервера
    /// </summary>
    void EndResponse();
  }

  public interface IClientEndpoint : IFactory<IClientMessage> { }

  public interface IServerEdnpoint : IFactory<IServerMessage>, IDisposable { }
}
