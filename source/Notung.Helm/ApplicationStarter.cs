using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Notung.Helm.Dialogs;
using Notung.Helm.Properties;
using Notung.Loader;
using Notung.Logging;
using Notung.Services;

namespace Notung.Helm
{
  /// <summary>
  /// Загрузчик Windows-приложения
  /// </summary>
  public class ApplicationStarter
  {
    private readonly IFactory<Form> m_form_factory;
    private readonly IFactory<ILoadingQueue> m_queue_factory;
    private IMainFormView m_view;

    private static SplashScreen _splash;
    private static string _splash_resource;

    private static readonly ILog _log = LogManager.GetLogger(typeof(ApplicationStarter));

    /// <summary>
    /// Создание загрузчика приложения с отдельной очередью загрузки
    /// </summary>
    /// <param name="mainFormFactory">Фабрика, порождающая главное окно приложения</param>
    /// <param name="loadingQueueFactory">Фабрика, порождающая очередь загрузки</param>
    public ApplicationStarter(IFactory<Form> mainFormFactory, IFactory<ILoadingQueue> loadingQueueFactory)
    {
      if (mainFormFactory == null)
        throw new ArgumentNullException("mainFormFactory");

      if (loadingQueueFactory == null)
        throw new ArgumentNullException("loadingQueueFactory");

      m_form_factory = mainFormFactory;
      m_queue_factory = loadingQueueFactory;
    }

    /// <summary>
    /// Создание загрузчика приложения без отдельной очереди загрузки
    /// </summary>
    /// <param name="mainFormFactory">Фабрика, порождающая главное окно приложения</param>
    public ApplicationStarter(IFactory<Form> mainFormFactory)
      : this(mainFormFactory, Factory.Empty<ILoadingQueue>()) { }

    /// <summary>
    /// Нужно ли разрешать только одну копию приложения
    /// </summary>
    public bool AllowOnlyOneInstance { get; set; }

    /// <summary>
    /// Изображение, показываемое во время загрузки (если перед загрузкой не был вызван метод ShowSplashScreen)
    /// </summary>
    public Image AlternativeSplashScreen { get; set; }

    /// <summary>
    /// Код возврата из приложения
    /// </summary>
    public int ExitCode { get; private set; }

    /// <summary>
    /// Показывает изображение при загрузке приложения. Рекомендуется этот метод вызывать в самом начале
    /// </summary>
    /// <param name="resource">Имя ресурса в сборке, запустившей приложение, откуда брать изображение</param>
    public static void ShowSplashScreen(string resource)
    {
      _splash = new SplashScreen(resource);
      _splash.Show(true);
      _splash_resource = resource;
    }

    /// <summary>
    /// Запуск приложения на выполнение
    /// </summary>
    /// <returns>Код возврата приложения</returns>
    public int RunApplication()
    {
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      Application.ThreadException += UIExceptionHandler;
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      this.ExcludePrefixes(AppManager.AssemblyClassifier.ExcludePrefixes.Add);

      m_view = this.CreateView(m_form_factory.Create());

      MainFormHandlers.Set(m_view, this);
      try
      {
        Application.Run(m_view.MainForm);
        _log.Debug("Application exited successfully");
      }
      catch (Exception ex)
      {
        _log.Error("Run(): exception", ex);
        this.ExitCode = -1;
      }

      return this.ExitCode;
    }

    #region Overridables --------------------------------------------------------------------------

    /// <summary>
    /// Создание представления на основе главной формы для всех сервисов приложения
    /// </summary>
    /// <param name="mainForm">Главная форма приложения</param>
    /// <returns>Представление для всех сервисов приложения</returns>
    protected virtual IMainFormView CreateView(Form mainForm)
    {
      return new MainFormView(mainForm);
    }

    /// <summary>
    /// Исключает префиксы внешних библиотек из классификатора сборок
    /// </summary>
    /// <param name="exclude">Метод, который надо вызвать, чтобы исключить префиксы</param>
    protected virtual void ExcludePrefixes(Action<string> exclude) { }

    #endregion

    #region Private methods -----------------------------------------------------------------------

    private Image GetSplashScreenImage()
    {
      if (_splash != null)
      {
        _splash.Close(TimeSpan.Zero);
        _splash = null;
      }

      if (!string.IsNullOrEmpty(_splash_resource))
      {
        var asm = Assembly.GetEntryAssembly();
        var name = new AssemblyName(asm.FullName);
        var rm = new ResourceManager(name.Name + ".g", asm);

        using (var ms = rm.GetStream(_splash_resource.ToLowerInvariant()))
        {
          return Image.FromStream(ms);
        }
      }
      else if (this.AlternativeSplashScreen != null)
        return this.AlternativeSplashScreen;
      else
        return Resources.DefaultSplash;
    }

    private ILoadingQueue GetLoadingQueue(object mainForm)
    {
      var queue1 = mainForm as ILoadingQueue;
      var queue2 = m_queue_factory.Create();

      if (queue1 == null)
        return queue2;

      if (queue2 == null)
        return queue1;

      return new LoadingQueueComposite(new ILoadingQueue[] { queue1, queue2 });
    }

    private void UIExceptionHandler(object sender, ThreadExceptionEventArgs e)
    {
      _log.Error("Application error", e.Exception);

      if (m_view != null && e.Exception != null)
      {
        m_view.ShowErrorBox(e.Exception.Message.Replace(Environment.NewLine, " "),
          string.Format(
              "{0}: {1}{2}{3}: {4}{2}{5}: {6}{2}{2}{7}:{2}{8}",
              Properties.Resources.EXCEPTION_TYPE, e.Exception.GetType(),
              Environment.NewLine,
              Properties.Resources.EXCEPTION_MESSAGE, e.Exception.Message,
              Properties.Resources.EXCEPTION_SOURCE, e.Exception.Source,
              Properties.Resources.EXCEPTION_CALLSTACK, e.Exception.StackTrace));
      }

      AppManager.Configurator.HandleError(e.Exception);
      AppManager.Configurator.SaveSettings();

      if (e.Exception is StackOverflowException)
      {
        this.ExitCode = -1;
        Application.Exit();
      }
    }

    #endregion

    #region Implementation ------------------------------------------------------------------------

    private class MainFormHandlers
    {
      private readonly IMainFormView m_view;
      private readonly ApplicationStarter m_starter;

      private MainFormHandlers(IMainFormView view, ApplicationStarter starter)
      {
        if (view == null)
          throw new ArgumentNullException("view");

        m_view = view;
        m_starter = starter;

        if (view.MainForm.WindowState == FormWindowState.Normal)
          view.MainForm.Shown += this.HandleMainFormLoad;
        else
          view.MainForm.Load += this.HandleMainFormLoad;

        view.MainForm.FormClosed += this.HandleClosed;
      }

      public static void Set(IMainFormView view, ApplicationStarter starter)
      {
        var handlers = new MainFormHandlers(view, starter);

        AppManager.Instance = new AppInstance(handlers.m_view);
        AppManager.Notificator = new Notificator(handlers.m_view);
        AppManager.OperationLauncher = new OperationLauncher(handlers.m_view);

        if (starter.AllowOnlyOneInstance)
          AppManager.Instance.AllowOnlyOneInstance();
      }

      private ApplicationLoadingResult RunLoadingWork(ApplicationLoadingWork work, Image splashScreen)
      {
        using (var dlg = m_view.GetSplashScreenView())
        {
          var presenter = new SplashScreenPresenter(work, dlg);
          presenter.Picture = splashScreen;

          dlg.ShowDialog(m_view.MainForm);
          return presenter.Result;
        }
      }

      private bool ShowSettingsForm(InfoBuffer buffer)
      {
        return m_view.ShowSettingsForm(buffer);
      }

      private void HandleMainFormLoad(object sender, EventArgs e)
      {
        var queue = m_starter.GetLoadingQueue(sender);

        if (queue == null)
          return;

        var image = m_starter.GetSplashScreenImage();
        var container = new DependencyContainer();
        var work = new ApplicationLoadingWork(queue, container);
        var res = this.RunLoadingWork(work, image);

        if (res == null)
        {
          _log.Fatal("InitApplication(): Can't get result loading task");
          m_starter.ExitCode = -1;
          Application.Exit();
          return;
        }

        while (!res.Success)
        {
          if (!AppManager.Notificator.Confirm(res.Buffer, Resources.INIT_FAIL_DESCRIPTION))
          {
            m_starter.ExitCode = 1;
            Application.Exit();
            return;
          }

          if (!this.ShowSettingsForm(res.Buffer))
          {
            m_starter.ExitCode = 2;
            Application.Exit();
            return;
          }

          work = new ApplicationLoadingWork(res, container);
          res = this.RunLoadingWork(work, image);
        }
      }

      private void HandleClosed(object sender, FormClosedEventArgs e)
      {
        AppManager.Configurator.SaveSettings();
      }
    }

    #endregion
  }
}