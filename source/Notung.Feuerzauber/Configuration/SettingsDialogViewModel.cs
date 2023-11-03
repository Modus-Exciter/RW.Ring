using Notung.Feuerzauber.Dialogs;

using Notung.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Notung.Feuerzauber.Configuration
{
   public class SettingsDialogViewModel : INotifyPropertyChanged
    {
        #region pivate
        private readonly ILog m_log;
        private readonly SettingsDialogService m_settingsDialogService;
        #endregion
        #region INotifyPropertyChanged 
        /// <summary>
        /// Событие возникающие при изменнеии значения свойсва
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Задать значение свойсва
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop">Имя свойсва</param>
        /// <param name="destination">Значение свойсва</param>
        /// <param name="newValue">Новое значение свойсва</param>
        /// <param name="changed">Дейсвие после изменения свойва</param>
        private void SetValue<T>(string prop, ref T destination, T newValue, Action<T> changed)
        {
            if ((newValue == null && destination != null) || (newValue != null && !newValue.Equals(destination)))
            {
                destination = newValue;
                OnPropertyChanged(prop);
                if(changed != null)
                changed.Invoke(destination);
            }
        }
        /// <summary>
        /// Вызывает событие изменения значения свойсва
        /// </summary>
        /// <param name="prop">Имя свойсва</param>
        private void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
        #region prorepty SettingsController
        private SettingsController m_settingsController;

        public SettingsController SettingsController { get => m_settingsController; set => SetValue("SettingsController", ref m_settingsController, value, SettingsControllerChanged); }


        private void SettingsControllerChanged(SettingsController  obj)
        {
            if (obj.ConfigurationPages != null && obj.ConfigurationPages != null && obj.ConfigurationPages.Count > 0)
            {
                this.СonfigurationPageSelected = obj.ConfigurationPages[0];

            }
            else
                this.СonfigurationPageSelected = null;
    
        }
        #endregion
        #region prorepty СonfigurationPageSelected
        private ConfigurationPage m_сonfigurationPageSelected;
        public ConfigurationPage СonfigurationPageSelected { get => m_сonfigurationPageSelected; set => 
                SetValue(nameof(СonfigurationPageSelected), ref m_сonfigurationPageSelected, value, СonfigurationPageSelectedChanged); }

        private void СonfigurationPageSelectedChanged(ConfigurationPage obj)
        {
            
        }
        #endregion

        #region prorepty СonfigurationPageSelected
   
        public ObservableCollection<ConfigurationPage> ConfigurationPagesList
        {
            get
            {
                if (m_settingsController != null && m_settingsController.ConfigurationPages != null)  return new ObservableCollection<ConfigurationPage>(m_settingsController.ConfigurationPages); 
                else return null;
            }
            
        }


        #endregion
        #region prorepty ValidationResults

        public BindingList<SettingsError> ValidationResults
        {
            get => m_settingsController.ValidationErrors;

        }


        #endregion
        #region prorepty ShowValidationResults
        private bool m_showValidationResults;
        public  bool ShowValidationResults
        {
            get => m_showValidationResults;

            set => this.SetValue("ShowValidationResults", ref m_showValidationResults, value, ShowValidationResultsChanged);
        }

        private void ShowValidationResultsChanged(bool obj)
        {
          
        }


        #endregion

        #region prorepty CanClearValidationResultSelection

        public bool CanClearValidationResultSelection
        {
            get => ValidationResultSelected != null;

        }


        #endregion

        #region prorepty ValidationResultSelected
        private SettingsError m_validationResultSelected;
        public SettingsError ValidationResultSelected
        {
            get => m_validationResultSelected;
            set => this.SetValue(nameof(ValidationResultSelected), ref m_validationResultSelected, value, ValidationResultsSelectedChanged);

        }
        private void ValidationResultsSelectedChanged(SettingsError obj)
        {

        }
        #endregion

        public SettingsDialogViewModel(SettingsDialogService settingsDialogService) : this(new SettingsController(), settingsDialogService) { }
        public SettingsDialogViewModel(SettingsController settingsController, SettingsDialogService settingsDialogService)
        {
            m_log = LogManager.GetLogger(typeof(SettingsDialogViewModel));
            m_settingsDialogService = settingsDialogService;
            m_settingsController = settingsController;
            m_settingsController.ValidationErrors.ListChanged += ValidationErrors_ListChanged; ;

            LoadedCommand = new RelayCommand(LoadedCommandAction, LoadedCommandCanExecute);
            OKCommand = new RelayCommand(OKCommandAction, OKCommandCanExecute);
            ApplyCommand = new RelayCommand(ApplyCommandAction, ApplyCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandAction, CancelCommandCanExecute);
            ValueChangedCommand = new RelayCommand(ValueChangedCommandAction, ValueChangedCommandCanExecute);
            ValidationResultSelectedCommand = new RelayCommand(ValidationResultSelectedCommandAction, ValidationResultSelectedCommandCanExecute);
        }

        private void ValidationErrors_ListChanged(object sender, ListChangedEventArgs e)
        {
            ShowValidationResults = ValidationResults.Count > 0;

         

            if (!ValidationResults.Contains(ValidationResultSelected))
                ValidationResultSelected = null;
        }


        #region Commands

        /// <summary>
        /// Команда вызываемая после загрузки окна 
        /// </summary>
        #region  LoadedCommand 
        public ICommand LoadedCommand { get; }
        private bool LoadedCommandCanExecute(object arg)
        {
            return true;
        }

        private void LoadedCommandAction(object obj)
        {
            m_settingsController.LoadAllPages();
            OnPropertyChanged("ConfigurationPagesList");
           
        }
        #endregion

        /// <summary>
        /// Команда нажатия кнопки Ok
        /// </summary>
        #region  OKCommand 
        public ICommand OKCommand { get; }
        private bool OKCommandCanExecute(object arg)
        {
            return /* !ShowValidationResults &&*/ SettingsController.ConfigurationPages.Any(x=>x.PageСhanged);
        }

        private void OKCommandAction(object obj)
        {

            if (m_settingsController.SaveAllSections()) 
                m_settingsDialogService.CloseDialog();
        }
        #endregion

        /// <summary>
        /// Команда нажатия кнопки Apply
        /// </summary>
        #region  ApplyCommand 
        public ICommand ApplyCommand { get; }
        private bool ApplyCommandCanExecute(object arg)
        {
            return /* !ShowValidationResults &&*/ SettingsController.ConfigurationPages.Any(x => x.PageСhanged);
        }

        private void ApplyCommandAction(object obj)
        {

            m_settingsController.SaveAllSections(true);
        }
        #endregion

        /// <summary>
        /// Двойное нажатие нажатия на ошбку
        /// </summary>
        #region  ValidationResultSelectedCommand 
        public ICommand ValidationResultSelectedCommand { get; }
        private bool ValidationResultSelectedCommandCanExecute(object arg)
        {
            return true;
        }

        private void ValidationResultSelectedCommandAction(object obj)
        {
            СonfigurationPageSelected = null;
            СonfigurationPageSelected = m_settingsController.ConfigurationPages.FirstOrDefault(x => {
                if (ValidationResultSelected == null)
                    return false;
                if (x.GetType() == ValidationResultSelected.SectionType)
                    return true;
                return false;
            });
        }
        #endregion
        /// <summary>
        /// Команда нажатия кнопки Cancel
        /// </summary>
        #region  CancelCommand 
        public ICommand CancelCommand { get; }
        private bool CancelCommandCanExecute(object arg)
        {
            return true;
        }

        private void CancelCommandAction(object obj)
        {
            m_settingsDialogService.CloseDialog();
        }
        #endregion

        /// <summary>
        /// Команда нажатия кнопки Cancel
        /// </summary>
        #region  ValueChangedCommand 
        public ICommand ValueChangedCommand { get; }
        private bool ValueChangedCommandCanExecute(object arg)
        {
            return true;
        }

        private void ValueChangedCommandAction(object obj)
        {
            m_settingsController.HandleSettingsChanged(this.СonfigurationPageSelected);
            this.OnPropertyChanged("ConfigurationPagesList");


        }
        #endregion
        #endregion


    }
    public class RelayCommand : ICommand
    {
        private Action<object> m_execute;
        private Func<object, bool> m_canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.m_execute = execute;
            this.m_canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.m_canExecute == null || this.m_canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.m_execute(parameter);
        }
    }

}
