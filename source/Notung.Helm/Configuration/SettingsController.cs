﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
  public sealed class SettingsController : Component, IListSource
  {
    private readonly BindingList<SettingsError> m_errors = new BindingList<SettingsError>();
    private readonly Dictionary<Type, IConfigurationPage> m_pages = new Dictionary<Type, IConfigurationPage>();
    private readonly Dictionary<Type, bool?> m_page_statuses = new Dictionary<Type, bool?>();
    private readonly SettingsBindingSourceCollection m_sections = new SettingsBindingSourceCollection();

    private static readonly object _skip_page_event_key = new object();
    private static readonly object _page_changed_event_key = new object();

    private static readonly ILog _log = LogManager.GetLogger(typeof(SettingsController));

    /// <summary>
    /// Инициализация нового экземпляра компонента для конфигурирования
    /// </summary>
    public SettingsController() : base() { }

    public SettingsController(IContainer container) : base()
    {
      if (container != null)
        container.Add(this);
    }

    #region Properties ----------------------------------------------------------------------------

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

    #endregion

    #region Events --------------------------------------------------------------------------------

    /// <summary>
    /// Происходит в момент загрузки страницы, позволяя отменить загрузку
    /// </summary>
    public event EventHandler<SkipPageEventArgs> SkipPage
    {
      add
      {
        this.Events.AddHandler(_skip_page_event_key, value);
      }
      remove
      {
        this.Events.RemoveHandler(_skip_page_event_key, value);
      }
    }

    /// <summary>
    /// Происходит, когда настройки на странице меняются
    /// </summary>
    public event EventHandler<PageEventArgs> PageChanged
    {
      add
      {
        this.Events.AddHandler(_page_changed_event_key, value);
      }
      remove
      {
        this.Events.RemoveHandler(_page_changed_event_key, value);
      }
    }

    #endregion

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
        AppManager.OperationLauncher.Invoker.GetType().Assembly);

      for (int i = 0; i < AppManager.AssemblyClassifier.TrackingAssemblies.Count; i++)
      {
        foreach (var type in AppManager.AssemblyClassifier
          .TrackingAssemblies[i].GetAvailableTypes(this.HandleTypeError))
        {
          if (!type.IsAbstract && typeof(IConfigurationPage).IsAssignableFrom(type)
            && type.GetConstructor(Type.EmptyTypes) != null)
          {
            var page = (IConfigurationPage)Activator.CreateInstance(type);

            if (!this.IsPageSkipped(page))
            {
              m_pages[type] = page;

              foreach (var section in page.Sections)
                section.LoadCurrentSettings();

              m_page_statuses[type] = null;
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

    private bool IsPageSkipped(IConfigurationPage page)
    {
      var args = new SkipPageEventArgs(page);

      (this.Events[_skip_page_event_key] as EventHandler<SkipPageEventArgs>).InvokeIfSubscribed(this, args);

      return args.Cancel;
    }

    private void HandleTypeError(Exception ex)
    {
      _log.Error("LoadAllPages(): exception", ex);
    }

    private void HandleSettingsChanged(object sender, EventArgs e)
    {
      var page = sender as IConfigurationPage;

      if (page == null)
        return;

      if (page.UIThreadValidation)
      {
        var buffer = new InfoBuffer();
        var page_valid = true;

        foreach (var section in page.Sections)
          page_valid = section.GetEditingSection().Validate(buffer) && page_valid;

        this.ReplaceErrors(page.GetType(), buffer);
        m_page_statuses[page.GetType()] = page_valid;

        if (this.ValidationResults != null)
          this.ValidationResults.Visible = m_errors.Count > 0;
      }
      else
        m_page_statuses[page.GetType()] = null;

      (this.Events[_page_changed_event_key] as EventHandler<PageEventArgs>).InvokeIfSubscribed(this, new PageEventArgs(page));
    }

    #endregion

    #region Sections validation -------------------------------------------------------------------

    private bool ValidateAllSections()
    {
      var backgrounds = new Dictionary<ConfigurationSection, Type>();
      var can_save = this.ValidateUIThreadSections(backgrounds);

      if (backgrounds.Count > 0)
        can_save = this.ValidateBackgroundThreadSections(backgrounds, can_save);

      if (this.ValidationResults != null)
        this.ValidationResults.Visible = m_errors.Count > 0;

      return can_save;
    }

    private bool ValidateUIThreadSections(Dictionary<ConfigurationSection, Type> backgrounds)
    {
      var can_save = true;

      foreach (var page in m_pages)
      {
        if (m_page_statuses[page.Key] != null)
        {
          can_save = can_save && m_page_statuses[page.Key].Value;
          continue;
        }

        if (page.Value.UIThreadValidation)
        {
          var page_valid = true;

          foreach (var settings in page.Value.Sections)
          {
            var buffer = new InfoBuffer();
            page_valid = settings.GetEditingSection().Validate(buffer) & page_valid;

            this.ReplaceErrors(page.Key, buffer);
          }

          m_page_statuses[page.Key] = page_valid;
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
        this.ReplaceErrors(kv.Key, kv.Value);

      foreach (var kv in wrk.PageResults)
        m_page_statuses[kv.Key] = kv.Value;

      return can_save;
    }

    private void ReplaceErrors(Type pageType, InfoBuffer buffer)
    {
      var deletee = new List<SettingsError>();

      foreach (var err in m_errors)
      {
        if (err.SectionType == pageType)
          deletee.Add(err);
      }

      foreach (var del in deletee)
        m_errors.Remove(del);

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

    private class ValidateSectionWork : RunBase
    {
      private readonly Dictionary<ConfigurationSection, Type> m_section;
      private readonly Dictionary<Type, InfoBuffer> m_results = new Dictionary<Type, InfoBuffer>();
      private readonly Dictionary<Type, bool> m_page_results = new Dictionary<Type, bool>();

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
            m_page_results[section.Value] = true;
          }

          var res = section.Key.Validate(buffer);

          m_page_results[section.Value] = m_page_results[section.Value] && res;

          this.Success = this.Success && res;
        }
      }

      public Dictionary<Type, InfoBuffer> Details
      {
        get { return m_results; }
      }

      public Dictionary<Type, bool> PageResults
      {
        get { return m_page_results; }
      }

      public override string ToString()
      {
        return Resources.VALIDATING_SECTIONS;
      }
    }

    #endregion
  }
}