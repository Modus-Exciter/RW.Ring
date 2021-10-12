using System;
using System.Reflection;
using Notung.Loader;
using Notung.Logging;

namespace Notung.Net
{
  public class ServerCaller
  {
    private readonly IFactory<object> m_factory;

    private static readonly ILog _log = LogManager.GetLogger(typeof(ServerCaller));

    public ServerCaller(IFactory<object> objectFactory)
    {
      if (objectFactory == null)
        throw new ArgumentNullException("objectFactory");

      m_factory = objectFactory;
    }

    private object Invoke(IParametersList request, RpcOperationInfo operation)
    {
      var values = request.GetValues();
      var result = operation.Method.Invoke(m_factory.Create(), values);

      if (operation.HasReferenceParameters)
      {
        var prs = operation.Method.GetParameters();

        for (int i = 0; i < prs.Length; i++)
        {
          if (!prs[i].ParameterType.IsByRef && !request.GetTypes()[i].IsValueType)
            values[i] = null;
        }

        if (operation.Method.ReturnType == typeof(void))
          return ParametersList.Create(operation.Method, values);

        var res = (IRefReturnResult)Activator.CreateInstance(operation.ResultType);
        res.References = ParametersList.Create(operation.Method, values);
        res.Return = result;

        return res;
      }
      else
        return result;
    }

    public ICallResult Call(RpcOperationInfo operation, IParametersList request)
    {
      var ret = (ICallResult)Activator.CreateInstance(operation.ResponseType);

      try
      {
        ret.Value = this.Invoke(request, operation);
      }
      catch (TargetInvocationException ex)
      {
        ret.Error = new ClientServerException(ex.InnerException.Message, ex.InnerException.StackTrace);
        _log.Error("Call(): TargetInvocationException", ex.InnerException ?? ex);
      }
      catch (Exception ex)
      {
        ret.Error = new ClientServerException(ex.Message, ex.StackTrace);
        _log.Error("Call(): Exception", ex);
      }

      return ret;
    }
  }
}