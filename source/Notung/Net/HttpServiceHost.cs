using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using Notung.Loader;
using Notung.Threading;

namespace Notung.Net
{
  public class HttpServiceHost : IDisposable
  {
    private Thread m_working_thread;
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

    public void AddService<T>(IFactory<T> creator) where T :class
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
          var bits = ReadBits(context);

          if (bits.Length == 1 && bits[0] == "favicon.ico")
          {
            context.Response.StatusCode = 200;
            return;
          }

          var info = RpcServiceInfo.GetByName(bits[0]);
          var parms = ReadParameters(context, info, bits[1]);

          ServerCaller caller;

          using (m_callers_lock.ReadLock())
            caller = m_callers[bits[0]];

          var result = caller.Call(bits, parms);

          if (result != null)
          {
            context.Response.ContentType = "application/json; Charset=utf-8";

            var return_type = ConversionHelper.GetResponseType(info.GetReturnType(bits[1]));
            var serializer = m_serialization.GetSerializer(return_type);

            serializer.Serialize(stream, result);
          }
        }
        catch (Exception ex)
        {
          context.Response.StatusCode = 400;
          context.Response.StatusDescription = ex.Message;

          using (var sw = new StreamWriter(context.Response.OutputStream))
          {
            sw.WriteLine("<html><body><h1>" + ex.Message + "</h1></body></html>");
          }
        }
        finally
        {
          ClientInfo.ThreadInfo = null;
          context.Response.Close();
        }
      }
    }

    private IParametersList ReadParameters(HttpListenerContext context, IRpcServiceInfo info, string methodName)
    {
      var params_type = info.GetParametersType(methodName);
      var method = info.GetMethod(methodName);
      var parameters = method.GetParameters();

      if (ConversionHelper.CanConvert(params_type))
      {
        var args = new object[parameters.Length];
        var query = ParseQueryString(context.Request.Url.Query);

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
            args[i] = TypeDescriptor.GetConverter(
              parameters[i].ParameterType).ConvertFromInvariantString(value);
        }

        return ParametersList.Create(method, args);
      }
      else if (parameters.Length > 0)
      {
        var req_serializer = m_serialization.GetSerializer(params_type);

        using (var input = context.Request.InputStream)
          return (IParametersList)req_serializer.Deserialize(input);
      }
      else
        return ParametersList.Create(method, Global.EmptyArgs);
    }

    private NameValueCollection ParseQueryString(string queryString)
    {
      var parameters = new NameValueCollection();

      foreach (string segment in queryString.Split('&'))
      {
        var index = segment.IndexOf('=');

        if (index >= 0)
        {
          parameters.Add(segment.Substring(0, index).Trim(new char[] { '?', ' ' }),
            Uri.UnescapeDataString(segment.Substring(index + 1)));
        }
      }

      return parameters;
    }

    private static string[] ReadBits(HttpListenerContext context)
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