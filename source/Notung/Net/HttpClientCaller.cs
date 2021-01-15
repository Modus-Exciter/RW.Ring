using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        var parameters = operation.Parameters;
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

        var serializer = m_factory.GetSerializer(request.GetType());

        using (var stream = web_request.GetRequestStream())
          serializer.Serialize(stream, request);
      }

      var response = web_request.GetResponse();

      using (var stream = response.GetResponseStream())
      {
        var return_type = HttpTypeHelper.GetResponseType(operation.ResponseType);
        var serializer = m_factory.GetSerializer(return_type);

        return (ICallResult)serializer.Deserialize(stream);
      }
    }

    public void StreamExchange(Action<Stream> processRequest, Action<Stream> processResponse)
    {
      if (processRequest == null)
        throw new ArgumentNullException("processRequest");

      if (processResponse == null)
        throw new ArgumentNullException("processResponse");

      var web_request = CreateWebRequest(string.Format("{0}/StreamExchange", m_base_url));
      web_request.Method = "POST";

      using (var stream = web_request.GetRequestStream())
        processRequest(stream);

      using (var response = web_request.GetResponse())
        processResponse(response.GetResponseStream());
    }

    public byte[] BinaryExchange(byte[] data)
    {
      var web_request = CreateWebRequest(string.Format("{0}/BinaryExchange", m_base_url));
      web_request.Method = "POST";

      if (data != null && data.Length > 0)
      {
        using (var stream = web_request.GetRequestStream())
          stream.Write(data, 0, data.Length);
      }

      using (var response = web_request.GetResponse())
      {
        List<byte> result = new List<byte>();
        byte[] buffer = new byte[512];

        using (var stream = response.GetResponseStream())
        {
          int count;

          while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
          {
            for (int i = 0; i < count; i++)
              result.Add(buffer[i]);
          }
        }

        return result.ToArray();
      }
    }

    private static HttpWebRequest CreateWebRequest(string builder)
    {
      var web_request = (HttpWebRequest)WebRequest.Create(builder.ToString());

      web_request.UseDefaultCredentials = true;
      web_request.UserAgent = ClientInfo.ProcessInfo.Application;
      web_request.Headers.Add("Machine-name", ClientInfo.ProcessInfo.MachineName);

      return web_request;
    }
  }
}