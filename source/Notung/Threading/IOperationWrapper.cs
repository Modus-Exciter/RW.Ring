﻿using System;
using System.ComponentModel;
using System.Threading;

namespace Notung.Threading
{
  /// <summary>
  /// Универсальный декоратор для выполнения произвольных операций
  /// </summary>
  public interface IOperationWrapper
  {
    /// <summary>
    /// Выполнение операции, возвращающей значение
    /// </summary>
    /// <typeparam name="TType">Тип возвращаемого значения</typeparam>
    /// <param name="action">Операция</param>
    /// <returns>Значение, которое вернула операция</returns>
    TType Invoke<TType>(Func<TType> action);

    /// <summary>
    /// Выполнение операции, не возвращающей значения
    /// </summary>
    /// <param name="action">Операция</param>
    void Invoke(Action action);
  }

  /// <summary>
  /// Декоратор операций, который не делает ничего
  /// </summary>
  public sealed class EmptyOperationWrapper : IOperationWrapper
  {
    public TType Invoke<TType>(Func<TType> action)
    {
      return action != null ? action() : default(TType);
    }

    public void Invoke(Action action)
    {
      if (action != null)
        action();
    }
  }

  /// <summary>
  /// Декоратор для выполнения операции в однопоточном апартаменте
  /// </summary>
  [Serializable]
  public sealed class ApartmentStateOperationWrapper : IOperationWrapper
  {
    public TType Invoke<TType>(Func<TType> action)
    {
      if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
      {
        TType ret = default(TType);
        RunInParallelThread(() => ret = action());
        return ret;
      }
      else
        return action();
    }

    public void Invoke(Action action)
    {
      if (action == null)
        return;

      if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
        RunInParallelThread(action.Invoke);
      else
        action();
    }

    private static void RunInParallelThread(ThreadStart threadStart)
    {
      Thread thread = new Thread(threadStart);

      thread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
      thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
      thread.SetApartmentState(ApartmentState.STA);
      thread.Priority = Thread.CurrentThread.Priority;
      thread.Start();
      thread.Join();
    }
  }


  /// <summary>
  /// Декоратор для прокидывания операций в поток пользовательского интерфейса
  /// через ISynchronizeInvoke
  /// </summary>
  public sealed class SynchronizeOperationWrapper : IOperationWrapper
  {
    private readonly ISynchronizeInvoke m_invoker;

    public SynchronizeOperationWrapper(ISynchronizeInvoke invoker, bool callProcedureAsync = false)
    {
      if (invoker == null)
        throw new ArgumentNullException("invoker");

      m_invoker = invoker;
      this.CallProceduresAsync = callProcedureAsync;
    }

    /// <summary>
    /// Вызывать методы, не возвращающие значения, через BeginInvoke, а не Invoke
    /// </summary>
    public bool CallProceduresAsync { get; set; }

    #region IOperationWrapper Members -------------------------------------------------------------

    public TType Invoke<TType>(Func<TType> action)
    {
      if (action == null)
        return default(TType);

      if (m_invoker.InvokeRequired)
        return (TType)m_invoker.Invoke(action, Global.EmptyArgs);
      else
        return action();
    }

    public void Invoke(Action action)
    {
      if (action == null)
        return;

      if (m_invoker.InvokeRequired)
      {
        if (this.CallProceduresAsync)
          m_invoker.BeginInvoke(action, Global.EmptyArgs);
        else
          m_invoker.Invoke(action, Global.EmptyArgs);
      }
      else
        action();
    }

    #endregion
  }
}