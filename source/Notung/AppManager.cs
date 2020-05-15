using System;
using Notung.Configuration;
using Notung.Loader;
using Notung.Log;
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
    private static ITaskManager _task_manager;

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

        _app_instance = value;
      }
    }

    /// <summary>
    /// Сведения о загруженных постоянных сборках и плагинах
    /// </summary>
    public static IAssemblyClassifier AssemblyClassifier
    {
      get
      {
        return _asm_classifier ?? InitService(ref _asm_classifier, () => new AssemblyClassifier());
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        lock (_lock)
        {
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
          _notificator.Dispose();
          _notificator = value;
        }
      }
    }

    /// <summary>
    /// Управление задачами с индикатором прогресса
    /// </summary>
    public static ITaskManager TaskManager
    {
      get
      {
        return _task_manager ?? InitService(ref _task_manager, () => new TaskManager());
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        _task_manager = value;
      }
    }
  }
}
