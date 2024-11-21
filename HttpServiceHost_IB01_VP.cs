using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Notung.Logging;
using Notung.Properties;
using Notung.Services;

namespace Notung.Net
{
  /// <summary>
  /// Хост для сервисов, работающих по протоколу HTTP
  /// </summary>
  public class HttpServiceHost : ServiceHostBase
  {
    private readonly HttpListener m_listener; 

    private static readonly ILog _log = LogManager.GetLogger(typeof(HttpServiceHost));

    public HttpServiceHost(ISerializationFactory serializationFactory, HttpListener listener)
      : base(serializationFactory)
    {
      if (serializationFactory == null)
        throw new ArgumentNullException("serializationFactory");

      m_listener = listener; 
    }

    protected override void StartListener()
    {
      m_listener.Start();
    }

    #region Override ------------------------------------------------------------------------------

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (!disposing)
        return;

      lock (m_lock)
      {
        m_listener.Stop();
        m_listener.Close();
      }
    }

    protected override object GetState()
    {
      return m_listener.GetContext();
    }

    protected override string PrepareCommand(string command)
    {
      return Uri.UnescapeDataString(command.Trim('?', ' '));
    }

    protected override void ProcessRequest(object state)
    {
      var context = (HttpListenerContext)state;
      ClientInfo.ThreadInfo = GetClientInfo(context);

      LogRequest(context);

      using (var stream = context.Response.OutputStream)
      {
        try
        {
          var bits = ParseLocalPath(context);

          if (bits.Length == 1)
          {
            if (this.ProcessSingleRequest(context, bits[0]))
              return;
          }

          if (bits.Length == 2)
          {
            var info = RpcServiceInfo.GetByName(bits[0]).GetOperationInfo(bits[1]);
            var result = this.GetCaller(bits[0]).Call(info, this.ReadParameters(context, info));

            if (result != null)
            {
              var serializer = this.GetSerializer(info.ResponseType);

              context.Response.ContentType = HttpTypeHelper.GetContentType(this.SerializationFormat);
              serializer.Serialize(stream, result);
            }
          }
          else
            throw new ArgumentOutOfRangeException("localPath", string.Format(
              Resources.INVALID_SERVER_OPERATION, context.Request.Url.LocalPath));
        }
        catch (Exception ex)
        {
          _log.Error("ProcessRequest(): exception", ex);

          context.Response.StatusCode = 400;
          context.Response.StatusDescription = Uri.EscapeDataString(ex.Message);
          var sw = new StreamWriter(context.Response.OutputStream);
          sw.WriteLine("<html><body><h1>{0}</h1></body></html>", ex.Message);
        }
        finally
        {
          ClientInfo.ThreadInfo = null;
          context.Response.Close();
        }
      }
    }

    #endregion

    #region Implementation ------------------------------------------------------------------------

    private bool ProcessSingleRequest(HttpListenerContext context, string request)
    {
      switch (request)
      {
        case "favicon.ico":
          context.Response.ContentType = "image/x-icon";
          Resources.DotChart.Save(context.Response.OutputStream);
          return true;

        case "BinaryExchange":
          return this.BinaryExchange(context.Request.Url.Query,
            context.Request.InputStream, context.Response.OutputStream);

        case "StreamExchange":
          return base.StreamExchange(context.Request.Url.Query,
            context.Request.InputStream, context.Response.OutputStream);

        case "ModelService": 
          WsdlGenerateService.HandleWsdlRequests(m_listener).GetAwaiter().GetResult();
          return true;
      }

      return false;
    }
     
    private IParametersList ReadParameters(HttpListenerContext context, RpcOperationInfo info)
    {
      var parameters = info.Method.GetParameters();

      if (parameters.Length > 0)
      {
        if (HttpTypeHelper.CanConvert(info.RequestType))
        {
          var args = new object[parameters.Length];
          var query = ParseQueryString(context.Request.Url.Query);
          var converter = HttpTypeHelper.GetConverter(info.RequestType);

          for (int i = 0; i < parameters.Length; i++)
          {
            var value = query[parameters[i].Name];

            if (value == null)
            {
              var par_type = parameters[i].ParameterType;

              if (par_type.IsByRef)
                par_type = par_type.GetElementType();

              if (par_type.IsValueType)
                args[i] = Activator.CreateInstance(par_type);
            }
            else
              args[i] = converter.ConvertFromString(value, i);
          }

          return ParametersList.Create(info.Method, args);
        }
        var req_serializer = this.GetSerializer(info.RequestType);

        using (var input = context.Request.InputStream)
          return (IParametersList)req_serializer.Deserialize(input);
      }
      else
        return ParametersList.Create(info.Method, Global.EmptyArgs);
    }

    private static void LogRequest(HttpListenerContext context)
    {
      string details = "";

      if (ClientInfo.ThreadInfo != null)
      {
        details = string.Format("{0}User: {1}, Application: {2}, Machine: {3}",
          Environment.NewLine,
          ClientInfo.ThreadInfo.UserName,
          ClientInfo.ThreadInfo.Application,
          ClientInfo.ThreadInfo.MachineName);
      }

      _log.Info(string.Format("{0} {1} {2}", context.Request.HttpMethod, context.Request.Url, details));
    }

    private static ClientInfo GetClientInfo(HttpListenerContext context)
    {
      if (context.User == null)
        return null;

      var application = context.Request.Headers["User-agent"];

      if (application == null)
        return null;

      var machine = context.Request.Headers["Machine-name"];

      if (machine == null)
        machine = context.Request.RemoteEndPoint.Address.ToString();

      return new ClientInfo
      {
        Application = application,
        UserName = context.User.Identity.Name,
        MachineName = machine
      };
    }

    private static NameValueCollection ParseQueryString(string queryString)
    {
      var parameters = new NameValueCollection();

      foreach (string segment in queryString.Split('&'))
      {
        var index = segment.IndexOf('=');

        if (index >= 0)
        {
          parameters.Add(segment.Substring(0, index).Trim(
            new char[] { '?', ' ' }), segment.Substring(index + 1));
        }
      }

      return parameters;
    }

    private static string[] ParseLocalPath(HttpListenerContext context)
    {
      var raw = context.Request.Url.LocalPath;

      if (raw.StartsWith("/"))
        raw = raw.Substring(1);

      if (raw.EndsWith("/"))
        raw = raw.Substring(0, raw.Length - 1);

      return raw.Split('/'); ;
    }

    #endregion
  }
}