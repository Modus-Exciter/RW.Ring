using System;
using System.Reflection;
using Notung.Loader;

namespace Notung.Net
{
  public class ServerCaller : IClientCaller
  {
    private readonly IFactory<object> m_factory;

    public ServerCaller(IFactory<object> objectFactory)
    {
      if (objectFactory == null)
        throw new ArgumentNullException("objectFactory");

      m_factory = objectFactory;
    }

    private object Invoke(IParametersList request, string[] bits, IRpcServiceInfo serviceInfo)
    {
      var values = request.GetValues();
      var method = serviceInfo.GetMethod(bits[1]);
      var result = method.Invoke(m_factory.Create(), values);

      if (serviceInfo.HasReferenceParameters(bits[1]))
      {
        var prs = method.GetParameters();

        for (int i = 0; i < prs.Length; i++)
        {
          if (!prs[i].ParameterType.IsByRef && !prs[i].ParameterType.IsValueType)
            values[i] = null;
        }

        if (method.ReturnType == typeof(void))
          return ParametersList.Create(method, values);

        IRefReturnResult res = (IRefReturnResult)Activator.CreateInstance(serviceInfo.GetReturnType(bits[1]));
        res.References = ParametersList.Create(method, values);
        res.Return = result;

        return res;
      }
      else
        return result;
    }

    public ICallResult Call(string [] bits, IParametersList request)
    {
      if (bits.Length != 2)
        throw new ArgumentException();

      var serviceInfo = RpcServiceInfo.GetByName(bits[0]);
      var return_type = ConversionHelper.GetResponseType(serviceInfo.GetReturnType(bits[1]));
      var ret = (ICallResult)Activator.CreateInstance(return_type);

      try
      {
        ret.Value = Invoke(request, bits, serviceInfo);
      }
      catch (TargetInvocationException ex)
      {
        ret.Error = new ApplicationException(ex.InnerException.Message);
      }
      catch (Exception ex)
      {
        ret.Error = new ApplicationException(ex.Message);
      }

      return ret;
    }

    ICallResult IClientCaller.Call(string serverOperation, IParametersList request, OperationInfo operation)
    {
      return this.Call(serverOperation.Split('/'), request);
    }
  }
}
