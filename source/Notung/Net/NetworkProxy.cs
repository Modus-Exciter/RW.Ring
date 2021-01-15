using System;
using System.Runtime.Remoting.Messaging;

namespace Notung.Net
{
  public class NetworkProxy<T> : GenericProxy<T> where T : class
  {
    private readonly IClientCaller m_caller;

    private static readonly IRpcServiceInfo _info = RpcServiceInfo<T>.Instance;

    public NetworkProxy(IClientCaller caller)
    {
      if (caller == null)
        throw new ArgumentNullException("caller");

      m_caller = caller;
      base.AddLocalService(caller);
    }

    protected override ReturnMessage Invoke(IMethodCallMessage message)
    {
      try
      {
        var method_name = _info.GetMethodName(message.MethodBase);
        var operation_info = _info.GetOperationInfo(method_name);

        var result = m_caller.Call(string.Format("{0}/{1}", _info.ServiceName, method_name),
          ParametersList.Create(message.MethodBase, message.Args), operation_info);

        if (result.Error != null)
          return new ReturnMessage(result.Error, message);
        else
        {
          if (operation_info.HasReferenceParameters)
          {
            IParametersList refs = (result.Value is IRefReturnResult) ?
              ((IRefReturnResult)result.Value).References : (IParametersList)result.Value;

            object ret = (result.Value is IRefReturnResult) ?
              ((IRefReturnResult)result.Value).Return : null;

            return new ReturnMessage(ret, refs.GetValues(),
              refs.GetTypes().Length, message.LogicalCallContext, message);
          }
          else
            return base.CreateReturnMesage(result.Value, message);
        }
      }
      catch (Exception ex)
      {
        return new ReturnMessage(ex, message);
      }
    }
  }
}
