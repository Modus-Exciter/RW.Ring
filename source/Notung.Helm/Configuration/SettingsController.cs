using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notung.ComponentModel;
using Notung.Configuration;
using Notung.Helm.Properties;
using Notung.Logging;
using Notung.Services;

namespace Notung.Helm.Configuration
{
  /// <summary>
  /// Компонент, в котором реализована основная логика формы настроек
  /// </summary>
  public class SettingsController : Component, IListSource
  {
    private readonly BindingList<SettingsError> m_errors = new BindingList<SettingsError>();
    private readonly Dictionary<Type, IConfigurationPage> m_pages = new Dictionary<Type, IConfigurationPage>();
    private readonly SettingsBindingSourceCollection m_sections = new SettingsBindingSourceCollection();

    private readonly ILog _log = LogManager.GetLogger(typeof(SettingsController));

    public SettingsController() : base() { }

    public SettingsController(IContainer container) : base()
    {
      if (container != null)
        container.Add(this);
    }

    /// <summary>
    /// Элемент управления, содержащий текущую секцию
    /// </summary>
    public Control PagePlace { get; set; }

    /// <summary>
    /// Элемент управления, содержащий результаты проверки секций
    /// </summary>
    public Control ValidationResults { get; set; }

    /// <summary>
    /// Ошибки в секциях. К этому свойству можно привязываться как к списку
    /// </summary>
    public IList<SettingsError> Errors
    {
      get { return m_errors; }
    }

    /// <summary>
    /// Страницы настроек
    /// </summary>
    public ICollection<IConfigurationPage> Pages
    {
      get { return m_pages.Values; }
    }

    /// <summary>
    /// Происходит в момент загрузки страницы, позволяя отменить загрузку
    /// </summary>
    public event EventHandler<SkipPageEventArgs> SkipPage
    {
      add
      {
        lock (this.Events)
          this.Events.AddHandler("SkipPage", value);
      }
      remove
      {
        lock (this.Events)
          this.Events.RemoveHandler("SkipPage", value);
      }
    }

    /// <summary>
    /// Выбор страницы настроек по типу
    /// </summary>
    /// <param name="pageType">Тип страницы, которую требуется активировать</param>
    public void SelectPage(Type pageType)
    {
      if (this.PagePlace == null)
        return;

      this.PagePlace.Controls.Clear();
      this.PagePlace.Controls.Add(m_pages[pageType].Page);
      m_pages[pageType].Page.Dock = DockStyle.Fill;
    }

    /// <summary>
    /// Загрузка всех конфигурационных секций, доступных в текущем домене
    /// </summary>
    public void LoadAllPages()
    {
      if (this.DesignMode)
        return;

      m_pages.Clear();
      m_sections.Clear();

      AppManager.AssemblyClassifier.LoadDependencies(
        AppManager.Instance.Invoker.GetType().Assembly);

      foreach (var asm in AppManager.AssemblyClassifier.TrackingAssemblies)
      {
        foreach (var type in GetAssemblyTypes(asm))
        {
          if (!type.IsAbstract && typeof(IConfigurationPage).IsAssignableFrom(type)
            && type.GetConstructor(Type.EmptyTypes) != null)
          {
            var page = (IConfigurationPage)Activator.CreateInstance(type);

            if (!IsPageSkipped(page))
              m_pages[page.GetType()] = page;

            foreach (var section in page.Sections)
              section.LoadCurrentSettings();
          }
        }
      }

      this.ValidateAllSections();
    }

    /// <summary>
    /// Сохранение всех настроек
    /// </summary>
    /// <param name="applyOnly">True, если требуется только применить настроки без сохранения. 
    /// False, если требуется применить и сохранить настройки</param>
    /// <returns>Успешность операции</returns>
    public bool SaveAllSections(bool applyOnly = false)
    {
      if (!this.ValidateAllSections())
        return false;
      
      foreach (var page in m_pages)
      {
        foreach (var section in page.Value.Sections)
        {
          section.GetEditingSection().ApplySettings();

          if (!applyOnly)
            section.SaveSettings();
        }
      }

      if (!applyOnly)
        AppManager.Configurator.SaveSettings();

      return true;
    }

    private bool IsPageSkipped(IConfigurationPage page)
    {
      var args = new SkipPageEventArgs(page);

      this.Events["SkipPage"].InvokeSynchronized(this, args);

      return args.Cancel;
    }

    #region Component model interfaces ------------------------------------------------------------

    bool IListSource.ContainsListCollection
    {
      get { return false; }
    }

    System.Collections.IList IListSource.GetList()
    {
      return m_errors;
    }

    private Type[] GetAssemblyTypes(Assembly assembly)
    {
      try
      {
        return assembly.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        _log.Error("GetAssemblyTypes(): exception", ex);
        return ex.Types.Where(t => t != null).ToArray();
      }
    }

    #endregion

    #region Sections validation -------------------------------------------------------------------

    private bool ValidateAllSections()
    {
      m_errors.Clear();

      var backgrounds = new Dictionary<ConfigurationSection, Type>();
      var can_save = ValidateUIThreadSections(backgrounds);

      if (backgrounds.Count > 0)
        can_save = ValidateBackgroundThreadSections(backgrounds, can_save);

      if (this.ValidationResults != null)
        this.ValidationResults.Visible = !can_save;

      return can_save;
    }

    private bool ValidateUIThreadSections(Dictionary<ConfigurationSection, Type> backgrounds)
    {
      var can_save = true;

      foreach (var page in m_pages)
      {
        if (page.Value.UIThreadValidation)
        {
          foreach (var settings in page.Value.Sections)
          {
            var buffer = new InfoBuffer();
            can_save = settings.GetEditingSection().Validate(buffer) & can_save;

            foreach (var info in buffer)
            {
              m_errors.Add(new SettingsError
              {
                Message = info.Message,
                SectionType = page.Key,
                Level = info.Level
              });
            }
          }
        }
        else
        {
          foreach (var settings in page.Value.Sections)
            backgrounds.Add(settings.GetEditingSection(), page.Key);
        }
      }
      return can_save;
    }

    private bool ValidateBackgroundThreadSections(Dictionary<ConfigurationSection, Type> backgrounds, bool can_save)
    {
      var wrk = new ValidateSectionWork(backgrounds);
      var launch = new LaunchParameters { Bitmap = Resources.Inspector };

      if (AppManager.OperationLauncher.Run(wrk, launch) != TaskStatus.RanToCompletion)
        can_save = false;

      can_save = wrk.Success && can_save;

      foreach (var kv in wrk.Details)
      {
        foreach (var info in kv.Value)
        {
          m_errors.Add(new SettingsError
          {
            Message = info.Message,
            SectionType = kv.Key,
            Level = info.Level
          });
        }
      }
      return can_save;
    }

    private class ValidateSectionWork : RunBase
    {
      private readonly Dictionary<ConfigurationSection, Type> m_section;
      private readonly Dictionary<Type, InfoBuffer> m_results = new Dictionary<Type, InfoBuffer>();

      public ValidateSectionWork(Dictionary<ConfigurationSection, Type> section)
      {
        m_section = section;
        this.Success = true;
      }

      public bool Success { get; private set; }

      public override void Run()
      {
        foreach (var section in m_section)
        {
          this.ReportProgress(string.Format(Resources.VALIDATING_SECTION, section.Key));

          InfoBuffer buffer;

          if (!m_results.TryGetValue(section.Value, out buffer))
          {
            buffer = new InfoBuffer();
            m_results.Add(section.Value, buffer);
          }

          this.Success = section.Key.Validate(buffer) && this.Success;
        }
      }

      public Dictionary<Type, InfoBuffer> Details
      {
        get { return m_results; }
      }

      public override string ToString()
      {
        return Resources.VALIDATING_SECTIONS;
      }
    }

    #endregion
  }

  /// <summary>
  /// Описание ошибки конфигурации, по которой можно перейти к нужной странице настроек
  /// </summary>
  public sealed class SettingsError
  {
    public string Message { get; set; }

    public InfoLevel Level { get; set; }

    [Browsable(false)]
    public Type SectionType { get; set; }
  }

  public class SkipPageEventArgs : CancelEventArgs
  {
    private readonly IConfigurationPage m_page;

    public SkipPageEventArgs(IConfigurationPage page) : base(false)
    {
      if (page == null)
        throw new ArgumentNullException("page");

      m_page = page;
    }

    public IConfigurationPage Page
    {
      get { return m_page; }
    }
  }
}