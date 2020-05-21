using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Notung.Helm.Properties;
using Notung.Loader;
using Notung.Logging;
using Notung.Threading;

namespace Notung.Helm
{
  public class ApplicationStarter
  {
    private readonly IFactory<Form> m_form_factory;
    private readonly IFactory<ILoadingQueue> m_loading_queue_factory;

    private static SplashScreen _splash;
    private static string _splash_resource;

    private static readonly ILog _log = LogManager.GetLogger(typeof(ApplicationStarter));

    public ApplicationStarter(IFactory<Form> formFactory, IFactory<ILoadingQueue> loadingQueueFactory)
    {
      if (formFactory == null)
        throw new ArgumentNullException("formFactory");

      if (loadingQueueFactory == null)
        throw new ArgumentNullException("loadingQueueFactory");

      m_form_factory = formFactory;
      m_loading_queue_factory = loadingQueueFactory;
    }

    public bool AllowOnlyOneInstance { get; set; }

    public Image AlternativeSplashScreen { get; set; }

    public static void ShowSplashScreen(string resource)
    {
      _splash = new System.Windows.SplashScreen(resource);
      _splash.Show(true);
      _splash_resource = resource;
    }

    public int Run()
    {
      System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      System.Windows.Forms.Application.ThreadException += UIExceptionHandler;
      System.Windows.Forms.Application.EnableVisualStyles();
      System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

      var main_form = m_form_factory.Create();

      SetMainFormHandlers(main_form);

      var view = this.CreateView(main_form);

      AppManager.Instance = new AppInstance(view);
      AppManager.Notificator = new Notificator(view);
      AppManager.OperationLauncher = new OperationLauncher(view);

      if (this.AllowOnlyOneInstance)
        AppManager.Instance.AllowOnlyOneInstance();

      this.ExcludePrefixes(AppManager.AssemblyClassifier.ExcludePrefixes.Add);

      try
      {
        Application.Run(main_form);
        _log.Debug("Application exited successfully");
        return 0;
      }
      catch (Exception ex)
      {
        _log.Error("Run(): exception", ex);
        return -1;
      }
    }

    protected virtual MainFormAppInstanceView CreateView(Form mainForm)
    {
      return new MainFormAppInstanceView(mainForm);
    }

    protected virtual void ExcludePrefixes(Action<string> exclude) { }

    protected virtual ApplicationLoadingResult RunLoadingTask(object sender, ApplicationLoadingTask task)
    {
      using (var dlg = new SplashScreenDialog(task))
      {
        dlg.Picture = this.GetSplashScreenImage();
        dlg.ShowDialog(sender as IWin32Window);

        return dlg.Result;
      }
    }

    protected virtual bool ShowSettingsForm(InfoBuffer buffer) { return true; }

    protected Image GetSplashScreenImage()
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
          return new Bitmap(Image.FromStream(ms));
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
      var queue2 = m_loading_queue_factory.Create();

      if (queue1 == null)
        return queue2;

      if (queue2 == null)
        return queue1;

      return new LoadingQueueComposite(new ILoadingQueue[] { queue1, queue2 });
    }

    private void SetMainFormHandlers(Form main_form)
    {
      if (main_form.WindowState == FormWindowState.Normal)
        main_form.Shown += this.HandleMainFormLoad;
      else
        main_form.Load += this.HandleMainFormLoad;

      main_form.FormClosed += this.HandleClosed;
    }

    private void UIExceptionHandler(object sender, ThreadExceptionEventArgs e)
    {
      _log.Error("Application error", e.Exception);
    }

    private void HandleMainFormLoad(object sender, EventArgs e)
    {
      var queue = this.GetLoadingQueue(sender);

      if (queue == null)
        return;

      var container = new DependencyContainer();
      var task = new ApplicationLoadingTask(queue, container);
      var res = this.RunLoadingTask(sender, task);

      if (res == null)
      {
        _log.Fatal("InitApplication(): Can't get result loading task");
        AppManager.Notificator.Show("FATAL ERROR", InfoLevel.Fatal);
        Application.Exit();
        return;
      }

      while (!res.Success)
      {
        if (!AppManager.Notificator.Confirm(res.Buffer, Resources.INIT_FAIL_DESCRIPTION))
          Application.Exit(); 

        if (!this.ShowSettingsForm(res.Buffer))
          Application.Exit();

        task = new ApplicationLoadingTask(res, container);
        res = this.RunLoadingTask(sender, task);
      }
    }

    private void HandleClosed(object sender, FormClosedEventArgs e)
    {
      AppManager.Configurator.SaveSettings();
    }
  }
}
