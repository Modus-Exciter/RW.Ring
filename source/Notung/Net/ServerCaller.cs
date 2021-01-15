using System;
using System.Reflection;
using Notung.Loader;

namespace Notung.Net
{
  public class ServerCaller
  {
    private readonly IFactory<object> m_factory;

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

        IRefReturnResult res = (IRefReturnResult)Activator.CreateInstance(operation.ResponseType);
        res.References = ParametersList.Create(operation.Method, values);
        res.Return = result;

        return res;
      }
      else
        return result;
    }

    internal ICallResult Call(RpcOperationInfo operation, IParametersList request)
    {
      var return_type = HttpTypeHelper.GetResponseType(operation.ResponseType);
      var ret = (ICallResult)Activator.CreateInstance(return_type);

      try
      {
        ret.Value = Invoke(request, operation);
      }
      catch (TargetInvocationException ex)
      {
        ret.Error = new ClientServerException(ex.InnerException.Message, ex.InnerException.StackTrace);
      }
      catch (Exception ex)
      {
        ret.Error = new ClientServerException(ex.Message, ex.StackTrace);
      }

      return ret;
    }

    public ICallResult Call(string [] bits, IParametersList request)
    {
      if (bits.Length != 2)
        throw new ArgumentException();

      var operation = RpcServiceInfo.GetByName(bits[0]).GetOperationInfo(bits[1]);

      return this.Call(operation, request);
    }
  }
}