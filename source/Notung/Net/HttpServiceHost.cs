using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading;
using Notung.Loader;
using Notung.Properties;
using Notung.Threading;

namespace Notung.Net
{
  public class HttpServiceHost : IDisposable
  {
    private Thread m_working_thread;
    private IBinaryService m_binary_service;
    private readonly object m_lock = new object();
    private readonly Dictionary<string, ServerCaller> m_callers = new Dictionary<string, ServerCaller>();
    private readonly SharedLock m_callers_lock = new SharedLock(false);
    private readonly ISerializationFactory m_serialization;
    private readonly HttpListener m_listener;

    public HttpServiceHost(ISerializationFactory serializationFactory, HttpListener listener)
    {
      if (serializationFactory == null)
        throw new ArgumentNullException("serializationFactory");

      if (listener == null)
        throw new ArgumentNullException("listener");

      m_serialization = serializationFactory;
      m_listener = listener;
    }

    public IBinaryService BinaryService
    {
      get { return m_binary_service; }
      set
      {
        using (m_callers_lock.WriteLock())
          m_binary_service = value;
      }
    }

    public void AddService<T>(IFactory<T> creator) where T : class
    {
      if (creator == null)
        throw new ArgumentNullException("creator");

      foreach (var contract in typeof(T).GetInterfaces())
      {
        if (!contract.IsDefined(typeof(RpcServiceAttribute), false))
          continue;

        using (m_callers_lock.WriteLock())
          m_callers.Add(RpcServiceInfo.Register(contract).ServiceName, new ServerCaller(creator));
      }

      using (m_callers_lock.WriteLock())
      {
        if (typeof(IBinaryService).IsAssignableFrom(typeof(T)) && m_binary_service == null)
          m_binary_service = new FactoryBinaryService(creator);
      }
    }

    public void Start()
    {
      lock (m_lock)
      {
        if (m_working_thread != null)
          throw new InvalidOperationException();
      }
      
      m_listener.Start();

      lock (m_lock)
      {
        m_working_thread = new Thread(ListeningThread);
        m_working_thread.Start();
      }
    }

    #region Destroy -------------------------------------------------------------------------------

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;

      lock (m_lock)
      {
        if (m_working_thread != null && m_working_thread.IsAlive)
        {
          m_working_thread.Abort();
          m_working_thread = null;
        }

        m_listener.Stop();
        m_listener.Close();
      }
    }

    #endregion

    #region Implementation ------------------------------------------------------------------------

    private void ListeningThread()
    {
      while (m_working_thread != null)
      {
        HttpListenerContext context = m_listener.GetContext();
        ThreadPool.QueueUserWorkItem(ProcessRequest, context);
      }
    }

    private void ProcessRequest(object state)
    {
      HttpListenerContext context = (HttpListenerContext)state;

      Console.WriteLine(context.Request.HttpMethod + " " + context.Request.Url);

      ClientInfo.ThreadInfo = GetClientInfo(context);

      if (ClientInfo.ThreadInfo != null)
      {
        Console.WriteLine("User: {0}, Application: {1}, Machine: {2}",
          ClientInfo.ThreadInfo.UserName, ClientInfo.ThreadInfo.Application, ClientInfo.ThreadInfo.MachineName);
      }

      Console.WriteLine();

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
            var result = GetCaller(bits[0]).Call(info, ReadParameters(context, info));

            if (result != null)
            {
              context.Response.ContentType = "application/json; Charset=utf-8";

              var return_type = HttpTypeHelper.GetResponseType(info.ResponseType);
              var serializer = m_serialization.GetSerializer(return_type);

              serializer.Serialize(stream, result);
            }
          }
          else
            throw new ArgumentOutOfRangeException();
        }
        catch (Exception ex)
        {
          context.Response.StatusCode = 400;
          context.Response.StatusDescription = ex.Message;

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

    private bool ProcessSingleRequest(HttpListenerContext context, string request)
    {
      switch (request)
      {
        case "favicon.ico":
          context.Response.ContentType = "image/x-icon";
          Resources.DotChart.Save(context.Response.OutputStream);
          return true;

        case "BinaryExchange":
          return BinaryExchange(context);

        case "StreamExchange":
          return StreamExchange(context);
      }

      return false;
    }

    private bool StreamExchange(HttpListenerContext context)
    {
      using (m_callers_lock.ReadLock())
      {
        if (m_binary_service != null)
        {
          m_binary_service.StreamExchange(context.Request.InputStream, context.Response.OutputStream);
          return true;
        }
        else
          return false;
      }
    }

    private bool BinaryExchange(HttpListenerContext context)
    {
      using (m_callers_lock.ReadLock())
      {
        if (m_binary_service != null)
        {
          List<byte> result = new List<byte>();
          byte[] buffer = new byte[512];
          int count;

          while ((count = context.Request.InputStream.Read(buffer, 0, buffer.Length)) > 0)
          {
            for (int i = 0; i < count; i++)
              result.Add(buffer[i]);
          }

          var ret = m_binary_service.BinaryExchange(result.ToArray());
          context.Response.OutputStream.Write(ret, 0, ret.Length);

          return true;
        }
        else
          return false;
      }
    }

    private ServerCaller GetCaller(string serviceName)
    {
      ServerCaller caller;

      using (m_callers_lock.ReadLock())
        caller = m_callers[serviceName];

      return caller;
    }

    private IParametersList ReadParameters(HttpListenerContext context, RpcOperationInfo info)
    {
      var parameters = info.Parameters;

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
        var req_serializer = m_serialization.GetSerializer(info.RequestType);

        using (var input = context.Request.InputStream)
          return (IParametersList)req_serializer.Deserialize(input);
      }
      else
        return ParametersList.Create(info.Method, Global.EmptyArgs);
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

      return raw.Split('/');;
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

    #endregion
  }
}