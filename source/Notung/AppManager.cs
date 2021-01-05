using System;
using System.Reflection;
using Notung.Configuration;
using Notung.Services;

namespace Notung
{
  /// <summary>
  /// Набор сервисов для управления приложением
  /// </summary>
  [AppDomainShare]
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

        lock (_lock)
        {
          if (ReferenceEquals(_notificator, value))
            return;

          if (value != null)
            ProcessUtil.SynchronizingObject = value.Invoker;

          _operation_launcher = value;
        }
      }
    }

    private static void Share(AppDomain newDomain)
    {
      var acceptor = (DomainAcceptor)newDomain.CreateInstanceAndUnwrap(
        Assembly.GetExecutingAssembly().FullName, typeof(DomainAcceptor).FullName);

      acceptor.AcceptServices(Instance, Configurator, Notificator, OperationLauncher);
    }

    private sealed class DomainAcceptor : MarshalByRefObject
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
