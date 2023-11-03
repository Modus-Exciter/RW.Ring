using Notung.Configuration;
using Notung.Feuerzauber.Properties;
using Notung.Logging;
using Notung.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Notung.Feuerzauber.Configuration
{
    /// <summary>
    /// Источник данных для формы настроен 
    /// </summary>
    public class SettingsController
    {

        #region pivate
        private readonly ILog m_log;
        private IConfigurator m_configurator;

        private readonly Dictionary<Type, bool?> m_page_statuses = new Dictionary<Type, bool?>();
        private readonly BindingList<SettingsError> m_errors = new BindingList<SettingsError>();

        #endregion
        public SettingsController(IConfigurator configurator)
        {
            m_log = LogManager.GetLogger(typeof(SettingsController));
            m_configurator = configurator;
        }

        public SettingsController() : this(AppManager.Configurator) { }

        #region Property ConfigurationPages
        public BindingList<ConfigurationPage> ConfigurationPages { get; private set; } = new BindingList<ConfigurationPage>();

        public BindingList<SettingsError> ValidationErrors { get => m_errors; }
        #endregion

        /// <summary>
        /// Метод загрузки страниц настройки 
        /// </summary>
        public void LoadAllPages()
        {
            AppManager.AssemblyClassifier.LoadDependencies(AppManager.OperationLauncher.Invoker.GetType().Assembly);
            ConfigurationPages.Clear();
            for (int i = 0; i < AppManager.AssemblyClassifier.TrackingAssemblies.Count; i++)
            {
                foreach (var type in AppManager.AssemblyClassifier
                  .TrackingAssemblies[i].GetAvailableTypes(this.HandleTypeError))
                {
                    if (!type.IsAbstract && typeof(ConfigurationPage).IsAssignableFrom(type)
                      && type.GetConstructor(Type.EmptyTypes) != null)
                    {
                        var page = (ConfigurationPage)Activator.CreateInstance(type);
                        List<Type> typesSections = new List<Type>();
                        page.InitializationSections(typesSections);
                        for(int j = 0; j < typesSections.Count; j++)
                        {
                            ConfigurationSection sel = CreateSectionCopy(typesSections[j]);
                           

                            page.Sections.Add(sel);
                        }
                        m_page_statuses[page.GetType()] = null;
                        ConfigurationPages.Add(page);

                       
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

            foreach (var page in ConfigurationPages)
            {
                foreach (var section in page.Sections)
                {
                    section.ApplySettings();

                    if (!applyOnly)
                    {
                        m_configurator.SaveSection(section);
                        page.PageСhanged = false;
                    }
                       
        
                }
            }

            if (!applyOnly)
            {
                m_configurator.SaveSettings();
            }
               

            return true;
        }
        /// <summary>
        /// Получение секции по типу текущего параметра из файла
        /// </summary>
        /// <param name="type"></param>
        /// <returns>ConfigurationSection</returns>
        private ConfigurationSection CreateSectionCopy(Type type)
        {
            var original = m_configurator.GetSection(type);

            using (var ms = new MemoryStream())
            {
                if (type.IsDefined(typeof(DataContractAttribute), false))
                {
                    var ser = new DataContractSerializer(type);
                    ser.WriteObject(ms, original);
                    ms.Position = 0;
                    return (ConfigurationSection)ser.ReadObject(ms);
                }
                else
                {
                    var ser = new XmlSerializer(type);
                    ser.Serialize(ms, original);
                    ms.Position = 0;
                    return (ConfigurationSection)ser.Deserialize(ms);
                }
            }
        }
        #region  Validation

        /// <summary>
        /// Выполнить валидацию всех секций
        /// </summary>
        /// <returns></returns>
        private bool ValidateAllSections()
        {
            var backgrounds = new Dictionary<ConfigurationSection, Type>();
            var can_save = this.ValidateUIThreadSections(backgrounds);

            if (backgrounds.Count > 0)
                can_save = this.ValidateBackgroundThreadSections(backgrounds, can_save);

            return can_save;
        }
        /// <summary>
        /// Удаление и замена ошибок
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="buffer"></param>
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
        /// <summary>
        /// Валидация в потоке UI
        /// </summary>
        /// <param name="backgrounds"></param>
        /// <returns></returns>
        private bool ValidateUIThreadSections(Dictionary<ConfigurationSection, Type> backgrounds)
        {
            var can_save = true;

            foreach (ConfigurationPage page in this.ConfigurationPages)
            {
                if (m_page_statuses[page.GetType()] != null)
                {
                    can_save = can_save && m_page_statuses[page.GetType()].Value;
                    continue;
                }

                if (page.UIThreadValidation)
                {
                    var page_valid = true;

                    foreach (var settings in page.Sections)
                    {
                        var buffer = new InfoBuffer();
                        page_valid = settings.Validate(buffer) & page_valid;

                        this.ReplaceErrors(page.GetType(), buffer);
                    }

                    m_page_statuses[page.GetType()] = page_valid;
                    can_save = can_save && page_valid;
                }
                else
                {
                    foreach (var settings in page.Sections)
                        backgrounds.Add(settings, page.GetType());
                }
            }

            return can_save;
        }
        /// <summary>
        /// Валидация асинхронно
        /// </summary>
        /// <param name="backgrounds"></param>
        /// <param name="can_save"></param>
        /// <returns></returns>
        private bool ValidateBackgroundThreadSections(Dictionary<ConfigurationSection, Type> backgrounds, bool can_save)
        {
            var wrk = new ValidateSectionWork(backgrounds);
            var launch = new LaunchParameters { /*Bitmap = Resources.Inspector*/ };

            if (AppManager.OperationLauncher.Run(wrk, launch) != TaskStatus.RanToCompletion)
                can_save = false;

            can_save = wrk.Success && can_save;

            foreach (var kv in wrk.Details)
                this.ReplaceErrors(kv.Key, kv.Value);

            foreach (var kv in wrk.PageResults)
                m_page_statuses[kv.Key] = kv.Value;

            return can_save;
        }
        /// <summary>
        /// Произошло изменение настроек 
        /// </summary>
        /// <param name="page"></param>
        public void HandleSettingsChanged(ConfigurationPage page)
        {
          

            if (page == null)
                return;

            if (page.UIThreadValidation)
            {
                var buffer = new InfoBuffer();
                var page_valid = true;

                foreach (var section in page.Sections)
                    page_valid = section.Validate(buffer) && page_valid;

                this.ReplaceErrors(page.GetType(), buffer);
                m_page_statuses[page.GetType()] = page_valid;

            
            }
            else
                m_page_statuses[page.GetType()] = null;

            page.PageСhanged = true;
         /*   (this.Events[_page_changed_event_key] as EventHandler<PageEventArgs>).InvokeIfSubscribed(this, new PageEventArgs(page));*/
        }
        private void HandleTypeError(Exception ex)
        {
            m_log.Error("LoadAllPages(): exception", ex);
        }
        #endregion
    }

    /*Данный класс взят как есть из Notung.Helm*/
    /// <summary>
    /// 
    /// </summary>
    public class ValidateSectionWork : RunBase
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
                this.ReportProgress(string.Format(/*Resources.VALIDATING_SECTION*/"Resources.VALIDATING_SECTION", section.Key));

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
            return /*Resources.VALIDATING_SECTIONS*/"Resources.VALIDATING_SECTION";
        }
    }

}
