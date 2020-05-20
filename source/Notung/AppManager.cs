#if APP_MANAGER
using System;
using System.Reflection;
using Notung.Configuration;
using Notung.Loader;
using Notung.Logging;
using Notung.Threading;

namespace Notung
{
  /// <summary>
  /// Набор сервисов для управления приложением
  /// </summary>
  public static class AppManager
  {
    private static IConfigurator _configurator;
    private static IAppInstance _app_instance;
    private static IAssemblyClassifier _asm_classifier;
    private static INotificator _notificator;
    private static IOperationLauncher _operation_launcher;

    private static readonly object _lock = new object();

    private static T InitService<T>(ref T field, Func<T> creator)
    {
      lock (_lock)
      {
        if (field == null)
          field = creator();

        return field;
      }
    }

    /// <summary>
    /// Конфигуратор для хранения пользовательских настроек, в том числе сложно структурированных
    /// </summary>
    public static IConfigurator Configurator
    {
      get
      {
        return _configurator ?? InitService(ref _configurator, () => new DataContractConfigurator());
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        lock (_lock) 
          _configurator = value;
      }
    }

    /// <summary>
    /// Сервис для управления текущим экземпляром приложения
    /// </summary>
    public static IAppInstance Instance
    {
      get
      {
        return _app_instance ?? InitService(ref _app_instance, () => new AppInstance());
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        lock (_lock) 
          _app_instance = value;
      }
    }

    /// <summary>
    /// Сведения о загруженных постоянных сборках и плагинах
    /// </summary>
    public static IAssemblyClassifier AssemblyClassifier
    {
      get { return _asm_classifier ?? InitService(ref _asm_classifier, () => new AssemblyClassifier()); }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        lock (_lock)
        {
          if (ReferenceEquals(_asm_classifier, value))
            return;

          if (_asm_classifier != null)
            _asm_classifier.Dispose();

          _asm_classifier = value;
        }
      }
    }

    /// <summary>
    /// Уведомления для оповещения пользователя о происходящих событиях
    /// </summary>
    public static INotificator Notificator
    {
      get
      {
        return _notificator ?? InitService(ref _notificator, () => new Notificator());
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        lock (_lock)
        {
          if (ReferenceEquals(_notificator, value))
            return;

          if (_notificator != null)
            _notificator.Dispose();

          _notificator = value;
        }
      }
    }

    /// <summary>
    /// Управление задачами с индикатором прогресса
    /// </summary>
    public static IOperationLauncher OperationLauncher
    {
      get { return _operation_launcher ?? InitService(ref _operation_launcher, () => new OperationLauncher()); }

      set
      {
        if (value == null)
          throw new ArgumentNullException();

        _operation_launcher = value;
      }
    }

    /// <summary>
    /// Настройка нового домена для работы с сервисами приложения. 
    /// Этот метод необходимо вызывать сразу после создания нового домена
    /// </summary>
    /// <param name="newDomain">Новый домен</param>
    public static void Share(AppDomain newDomain)
    {
      if (newDomain == null)
        throw new ArgumentNullException("newDomain");

      if (newDomain == AppDomain.CurrentDomain)
        return;

      var acceptor = (IDomainAcceptor)newDomain.CreateInstanceAndUnwrap(
        Assembly.GetExecutingAssembly().FullName, typeof(DomainAcceptor).FullName);

      acceptor.AcceptServices(Instance, Configurator, Notificator, OperationLauncher);

      LogManager.Share(newDomain);
      LoggingContext.Share(newDomain);
#if APPLICATION_INFO
      ApplicationInfo.Share(newDomain);
#endif
    }

    private interface IDomainAcceptor
    {
      void AcceptServices(IAppInstance instance, 
                          IConfigurator configurator, 
                          INotificator notificator, 
                          IOperationLauncher operationLauncher);
    }

    private sealed class DomainAcceptor : MarshalByRefObject, IDomainAcceptor
    {
      public void AcceptServices(IAppInstance instance, 
                                 IConfigurator configurator, 
                                 INotificator notificator,
                                 IOperationLauncher operationLauncher)
      {
        _notificator = notificator;
        _app_instance = instance;
        _configurator = configurator;
        _operation_launcher = new OperationLauncherProxy(operationLauncher);
      }
    }
  }
}
#endif