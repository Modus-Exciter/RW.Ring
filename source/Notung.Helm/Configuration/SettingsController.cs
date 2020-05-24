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
    private readonly Dictionary<Type, ValidationStatus> m_page_statuses = new Dictionary<Type, ValidationStatus>();
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
            {
              m_pages[type] = page;

              foreach (var section in page.Sections)
                section.LoadCurrentSettings();

              m_page_statuses[type] = new ValidationStatus();
              page.Changed += this.HandleSettingsChanged;
            }
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

    #region IListSource members -------------------------------------------------------------------

    bool IListSource.ContainsListCollection
    {
      get { return false; }
    }

    System.Collections.IList IListSource.GetList()
    {
      return m_errors;
    }

    #endregion

    #region Private methods -----------------------------------------------------------------------

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

    private void HandleSettingsChanged(object sender, EventArgs e)
    {
      var page = sender as IConfigurationPage;

      if (page == null)
        return;

      m_page_statuses[page.GetType()].Changed = true;

      if (page.UIThreadValidation)
      {
        var deletee = new List<SettingsError>();

        foreach (var err in m_errors)
        {
          if (err.SectionType == page.GetType())
            deletee.Add(err);
        }

        foreach (var del in deletee)
          m_errors.Remove(del);

        var buffer = new InfoBuffer();
        var page_valid = true;

        foreach (var section in page.Sections)
          page_valid = section.GetEditingSection().Validate(buffer) && page_valid;

        this.AddErrors(page.GetType(), buffer);
        m_page_statuses[page.GetType()].Valid = page_valid;

        if (this.ValidationResults != null)
          this.ValidationResults.Visible = m_errors.Count > 0;
      }
      else
        m_page_statuses[page.GetType()] = null;
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
        this.ValidationResults.Visible = m_errors.Count > 0;

      return can_save;
    }

    private bool ValidateUIThreadSections(Dictionary<ConfigurationSection, Type> backgrounds)
    {
      var can_save = true;

      foreach (var page in m_pages)
      {
        if (page.Value.UIThreadValidation)
        {
          var page_valid = true;
          
          foreach (var settings in page.Value.Sections)
          {
            var buffer = new InfoBuffer();
            page_valid = settings.GetEditingSection().Validate(buffer) & page_valid;

            this.AddErrors(page.Key, buffer);
          }

          m_page_statuses[page.Key].Valid = page_valid;
          can_save = can_save && page_valid;
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
        this.AddErrors(kv.Key, kv.Value);

      return can_save;
    }

    private void AddErrors(Type pageType, InfoBuffer buffer)
    {
      foreach (var info in buffer)
      {
        m_errors.Add(new SettingsError
        {
          Message = info.Message,
          SectionType = pageType,
          Level = info.Level
        });
      }
    }

    private class ValidationStatus
    {
      public bool Changed { get; set; }

      public bool? Valid { get; set; }
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
    /// <summary>
    /// Соообщение об ошибке
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Уровень сообщения
    /// </summary>
    public InfoLevel Level { get; set; }

    /// <summary>
    /// Тип конфигурационной секции, к которой привязано сообщение
    /// </summary>
    [Browsable(false)]
    public Type SectionType { get; set; }
  }

  /// <summary>
  /// Аргументы события, возникающего передс созданием страницы настроек
  /// </summary>
  public class SkipPageEventArgs : CancelEventArgs
  {
    private readonly IConfigurationPage m_page;

    public SkipPageEventArgs(IConfigurationPage page) : base(false)
    {
      if (page == null)
        throw new ArgumentNullException("page");

      m_page = page;
    }

    /// <summary>
    /// Создаваемая страница настроек
    /// </summary>
    public IConfigurationPage Page
    {
      get { return m_page; }
    }
  }
}