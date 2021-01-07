using System;
using System.ComponentModel;
using Notung.Properties;

namespace Notung.ComponentModel
{
  public static partial class ComponentExtensions
  {
    /// <summary>
    /// Проверяет, изменилось ли указанное свойство в событии PropertyChanged
    /// </summary>
    /// <param name="property">Свойство, которое проверяется</param>
    /// <returns>True, если свойство изменилось. Иначе, false</returns>
    public static bool IsChanged(this PropertyChangedEventArgs e, string property)
    {
      return string.IsNullOrEmpty(e.PropertyName) || e.PropertyName.Equals(property);
    }

    /// <summary>
    /// Получает значение указанного типа от IServiceProvider
    /// </summary>
    /// <typeparam name="TService">Тип, который требуется получить</typeparam>
    /// <returns>Значение указанного типа, если IServiceProvider поддерживает этот тип</returns>
    public static TService GetService<TService>(this IServiceProvider provider) where TService: class
    {
      return provider.GetService(typeof(TService)) as TService;
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this EventHandler handler, object sender, EventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this AddingNewEventHandler handler, object sender, AddingNewEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this AsyncCompletedEventHandler handler, object sender, AsyncCompletedEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this CancelEventHandler handler, object sender, CancelEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this CollectionChangeEventHandler handler, object sender, CollectionChangeEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this DoWorkEventHandler handler, object sender, DoWorkEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this HandledEventHandler handler, object sender, HandledEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this ListChangedEventHandler handler, object sender, ListChangedEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this ProgressChangedEventHandler handler, object sender, ProgressChangedEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this PropertyChangedEventHandler handler, object sender, PropertyChangedEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this PropertyChangingEventHandler handler, object sender, PropertyChangingEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed(this RunWorkerCompletedEventHandler handler, object sender, RunWorkerCompletedEventArgs e)
    {
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Выполняет обработчик события, если на него есть подписчики
    /// </summary>
    /// <typeparam name="TArgs">Тип аргумента события</typeparam>
    /// <param name="handler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    public static void InvokeIfSubscribed<TArgs>(this EventHandler<TArgs> handler, object sender, TArgs e)
      where TArgs : EventArgs
    {
      if (handler != null)
        handler(sender, e);
    }
  }
}
