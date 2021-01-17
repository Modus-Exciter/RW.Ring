using System;
using System.IO;
using System.Net;
using System.Text;

namespace Notung.Net
{
  public class HttpClientCaller : IClientCaller
  {
    private readonly ISerializationFactory m_factory;
    private readonly string m_base_url;

    public HttpClientCaller(string baseUrl, ISerializationFactory serializationFactory)
    {
      if (serializationFactory == null)
        throw new ArgumentNullException("serializationFactory");
      
      m_base_url = baseUrl.Trim();
      m_factory = serializationFactory;

      if (m_base_url[m_base_url.Length - 1] != '/')
        m_base_url += "/";
    }

    public ICallResult Call(string serverOperation, IParametersList request, RpcOperationInfo operation)
    {
      var builder = new StringBuilder(m_base_url);
      builder.Append(serverOperation);

      if (request.GetTypes().Length > 0 && HttpTypeHelper.CanConvert(request.GetType()))
      {
        var parameters = operation.Method.GetParameters();
        var args = request.GetValues();
        var converter = HttpTypeHelper.GetConverter(request.GetType());

        builder.AppendFormat("/?{0}={1}", parameters[0].Name, converter.ConvertToString(args[0], 0));

        for (int i = 1; i < parameters.Length; i++)
          builder.AppendFormat("&{0}={1}", parameters[i].Name, converter.ConvertToString(args[i], i));
      }

      var web_request = CreateWebRequest(builder.ToString());

      if (request.GetTypes().Length > 0 && !HttpTypeHelper.CanConvert(request.GetType()))
      {
        web_request.Method = "POST";
        web_request.ContentType = HttpTypeHelper.GetContentType(m_factory.Format);

        var serializer = m_factory.GetSerializer(request.GetType());

        using (var stream = web_request.GetRequestStream())
          serializer.Serialize(stream, request);
      }

      var response = web_request.GetResponse();

      using (var stream = response.GetResponseStream())
      {
        var return_type = operation.ResponseType;
        var serializer = m_factory.GetSerializer(return_type);

        return (ICallResult)serializer.Deserialize(stream);
      }
    }

    public void StreamExchange(string command, Action<Stream> processRequest, Action<Stream> processResponse)
    {
      if (processRequest == null)
        throw new ArgumentNullException("processRequest");

      if (processResponse == null)
        throw new ArgumentNullException("processResponse");

      var web_request = CreateWebRequest(string.Format("{0}/StreamExchange/?{1}",
        m_base_url, Uri.EscapeDataString(command)));

      web_request.Method = "POST";
      web_request.ContentType = HttpTypeHelper.GetContentType(m_factory.Format);

      using (var stream = web_request.GetRequestStream())
        processRequest(stream);

      using (var response = web_request.GetResponse())
        processResponse(response.GetResponseStream());
    }

    public byte[] BinaryExchange(string command, byte[] data)
    {
      var web_request = CreateWebRequest(string.Format("{0}/BinaryExchange?{1}",
        m_base_url, Uri.EscapeDataString(command)));

      web_request.Method = "POST";
      web_request.ContentType = HttpTypeHelper.GetContentType(m_factory.Format);

      if (data != null && data.Length > 0)
      {
        using (var stream = web_request.GetRequestStream())
          stream.Write(data, 0, data.Length);
      }

      using (var response = web_request.GetResponse())
      {
        using (var stream = response.GetResponseStream())
        {
          using (var memory_stream = new MemoryStream())
          {
            stream.CopyTo(memory_stream);
            return memory_stream.ToArray();
          }
        }
      }
    }

    private HttpWebRequest CreateWebRequest(string builder)
    {
      var web_request = (HttpWebRequest)WebRequest.Create(builder.ToString());

      web_request.UseDefaultCredentials = true;
      web_request.UserAgent = ClientInfo.ProcessInfo.Application;
      web_request.Headers.Add("Machine-name", ClientInfo.ProcessInfo.MachineName);

      return web_request;
    }
  }
}