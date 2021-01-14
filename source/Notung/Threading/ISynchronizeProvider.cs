using System;
using System.ComponentModel;

namespace Notung.Threading
{
  /// <summary>
  /// Базовый тип для интерфейсов, предоставляющих доступ к объекту синхронизации
  /// </summary>
  public interface ISynchronizeProvider
  {
    /// <summary>
    /// Объект синхронизации
    /// </summary>
    ISynchronizeInvoke Invoker { get; }
  }

  /// <summary>
  /// Предоставляет доступ к пустому объекту синхронизации
  /// </summary>
  public class EmptySynchronizeProvider : ISynchronizeProvider
  {
    [Serializable]
    private class Synchronizer : ISynchronizeInvoke
    {
      public IAsyncResult BeginInvoke(Delegate method, object[] args)
      {
        var func = new Func<object[], object>(method.DynamicInvoke);
        return func.BeginInvoke(args, null, func);
      }

      public object EndInvoke(IAsyncResult result)
      {
        return ((Func<object[], object>)result.AsyncState).EndInvoke(result);
      }

      public object Invoke(Delegate method, object[] args)
      {
        return method.DynamicInvoke(args);
      }

      public bool InvokeRequired
      {
        get { return false; }
      }
    }

    public static readonly ISynchronizeInvoke Default = new Synchronizer();

    /// <summary>
    /// Объект синхронизации, не меняющий поток (работает как базовый SynchronizationContext)
    /// </summary>
    public ISynchronizeInvoke Invoker
    {
      get { return Default; }
    }
  }
}
