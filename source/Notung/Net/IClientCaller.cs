using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Notung.Net
{
  public interface IClientCaller
  {
    ICallResult Call(string serverOperation, IParametersList request, OperationInfo operation);
  }

  public interface ICallResult
  {
    object Value { get; set; }

    Exception Error { get; set; }
  }

  public struct OperationInfo
  {
    public Type ReturnType { get; set; }

    public ParameterInfo[] Parameters { get; set; }
  }

  [Serializable, DataContract(Name = "RES", Namespace = "")]
  internal class CallResult<TResult> : ICallResult
  {
    [DataMember(Name = "Res")]
    private TResult m_result;
    [DataMember(Name = "Err")]
    private ApplicationException m_error;

    public object Value
    {
      get { return m_result; }
      set { m_result = (TResult)value; }
    }

    public Exception Error
    {
      get { return m_error; }
      set { m_error = (ApplicationException)value; }
    }
  }

  public class WebRequestClientCaller : IClientCaller
  {
    private readonly ISerializationFactory m_factory;
    private readonly string m_base_url;

    public WebRequestClientCaller(string baseUrl, ISerializationFactory serializationFactory)
    {
      m_base_url = baseUrl;
      m_factory = serializationFactory;
    }
    
    public ICallResult Call(string serverOperation, IParametersList request, OperationInfo operation)
    {
      var builder = new StringBuilder(m_base_url);

      if (m_base_url.Last() != '/')
        builder.Append('/');

      builder.Append(serverOperation);

      if (request.GetTypes().Length > 0 && ConversionHelper.CanConvert(request.GetType()))
      {
        var parameters = operation.Parameters;
        var args = request.GetValues();

        builder.Append("/?");

        builder.AppendFormat("{0}={1}", parameters[0].Name, Uri.EscapeDataString(
          TypeDescriptor.GetConverter(parameters[0].ParameterType).ConvertToInvariantString(args[0])));

        for (int i = 1; i < parameters.Length; i++)
          builder.AppendFormat("&{0}={1}", parameters[i].Name, Uri.EscapeDataString(
            TypeDescriptor.GetConverter(parameters[i].ParameterType).ConvertToInvariantString(args[i])));
      }

      var web_request = (HttpWebRequest)WebRequest.Create(builder.ToString());

      web_request.UseDefaultCredentials = true;
      web_request.UserAgent = ClientInfo.ProcessInfo.Application;
      web_request.Headers.Add("Machine-name", ClientInfo.ProcessInfo.MachineName);

      if (request.GetTypes().Length > 0 && !ConversionHelper.CanConvert(request.GetType()))
      {
        web_request.Method = "POST";

        var serializer = m_factory.GetSerializer(request.GetType());

        using (var stream = web_request.GetRequestStream())
          serializer.Serialize(stream, request);
      }

      var response = web_request.GetResponse();

      using (var stream = response.GetResponseStream())
      {
        var return_type = ConversionHelper.GetResponseType(operation.ReturnType);
        var serializer = m_factory.GetSerializer(return_type);

        return (ICallResult)serializer.Deserialize(stream);
      }
    }
  }
}
