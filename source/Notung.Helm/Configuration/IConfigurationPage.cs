using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using Notung.Configuration;

namespace Notung.Helm.Configuration
{
  /// <summary>
  /// Интерфейс одной страницы настроек
  /// </summary>
  public interface IConfigurationPage
  {
    /// <summary>
    /// Конфигурационные секции, редактируемые на данной странице
    /// </summary>
    SettingsBindingSourceCollection Sections { get; }

    /// <summary>
    /// Страница настроек
    /// </summary>
    Control Page { get; }

    /// <summary>
    /// Картинка в списке настроек
    /// </summary>
    Image Image { get; }

    /// <summary>
    /// Выполнять ли проверку настроек в потоке пользовательского интерфейса
    /// </summary>
    bool UIThreadValidation { get; }
  }

  /// <summary>
  /// Набор конфигурационных секций, редактируемых на одной странице настроек
  /// </summary>
  public sealed class SettingsBindingSourceCollection : KeyedCollection<Type, SettingsBindingSource>
  {
    protected override Type GetKeyForItem(SettingsBindingSource item)
    {
      return item.GetType().GetGenericArguments()[0];
    }

    public SettingsBindingSource<TSection> Get<TSection>()
      where TSection : ConfigurationSection, new()
    {
      return this[typeof(TSection)] as SettingsBindingSource<TSection>;
    }
  }

  /// <summary>
  /// Обёртка над конфигурационной секцией, редактируемая через графический интерфейс
  /// </summary>
  public abstract class SettingsBindingSource
  {
    internal SettingsBindingSource() { }

    public abstract void LoadCurrentSettings();

    public abstract void SaveSettings();

    public abstract void RestoreDefaults();

    public abstract ConfigurationSection GetEditingSection();
  }

  /// <summary>
  /// Привязывает конфигурационную секцию к компоненту BidningSource на форме
  /// </summary>
  /// <typeparam name="TSection">Тип привязываемой конфигурационной секции</typeparam>
  public sealed class SettingsBindingSource<TSection> : SettingsBindingSource
    where TSection : ConfigurationSection, new()
  {
    private readonly IConfigurator m_configurator;
    private readonly BindingSource m_binding_source;
    private bool m_getting_settings;

    public SettingsBindingSource(BindingSource bindingSource)
      : this(AppManager.Configurator, bindingSource) { }

    public SettingsBindingSource(IConfigurator configurator, BindingSource bindingSource)
    {
      if (configurator == null)
        throw new ArgumentNullException("configurator");

      if (bindingSource == null)
        throw new ArgumentNullException("bindingSource");

      if (bindingSource.DataSource != null)
      {
        var type = bindingSource.DataSource as Type;

        if (type == null)
          type = bindingSource.DataSource.GetType();

        if (type != typeof(TSection))
          throw new ArgumentOutOfRangeException("bindingSource", type.FullName,
            string.Format("bindingSource.DataSource {0} expected", typeof(TSection).FullName));
      }

      m_configurator = configurator;
      m_binding_source = bindingSource;
      m_binding_source.DataSourceChanged += this.HandleDataSourceChanged;
    }

    /// <summary>
    /// Экземпляр редактируемой конфигурационной секции
    /// </summary>
    public TSection EditingSection
    {
      get { return m_binding_source.DataSource as TSection; }
    }

    /// <summary>
    /// Получение редактируемой секции для компонента загрузки
    /// </summary>
    /// <returns>Экземпляр редактируемой конфигурационной секции</returns>
    public override ConfigurationSection GetEditingSection()
    {
      return this.EditingSection;
    }

    /// <summary>
    /// Помещение конфигурационной секции на форму
    /// </summary>
    public override void LoadCurrentSettings()
    {
      m_getting_settings = true;
      try
      {
        m_binding_source.DataSource = this.CreateSectionCopy();
      }
      finally
      {
        m_getting_settings = false;
      }
    }

    /// <summary>
    /// Сохранение отредактированного экземпляра секции в конфигуратор
    /// </summary>
    public override void SaveSettings()
    {
      if (this.EditingSection == null)
        throw new InvalidOperationException("EditingSection = null");

      m_configurator.SaveSection(this.EditingSection);
    }

    /// <summary>
    /// Восстановление настроек по умолчанию.
    /// </summary>
    public override void RestoreDefaults()
    {
      this.EditingSection.RestoreDefaults();
      this.m_binding_source.ResetBindings(false);
    }

    private void HandleDataSourceChanged(object sender, EventArgs e)
    {
      if (!m_getting_settings)
        throw new InvalidOperationException("¬m_getting_settings");
    }

    private TSection CreateSectionCopy()
    {
      var original = m_configurator.GetSection<TSection>();

      using (var ms = new MemoryStream())
      {
        if (typeof(TSection).IsDefined(typeof(DataContractAttribute), false))
        {
          var ser = new DataContractSerializer(typeof(TSection));

          ser.WriteObject(ms, original);

          ms.Position = 0;

          return (TSection)ser.ReadObject(ms);
        }
        else
        {
          var ser = new XmlSerializer(typeof(TSection));

          ser.Serialize(ms, original);

          ms.Position = 0;

          return (TSection)ser.Deserialize(ms);
        }
      }
    }
  }
}
