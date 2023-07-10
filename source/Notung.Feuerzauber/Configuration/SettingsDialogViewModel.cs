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
        private readonly ILog _log;
        private readonly SettingsDialogService _settingsDialogService;
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
                changed?.Invoke(destination);
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
        private SettingsController _settingsController;

        public SettingsController SettingsController { get => _settingsController; set => SetValue("SettingsController", ref _settingsController, value, SettingsControllerChanged); }


        private void SettingsControllerChanged(SettingsController  obj)
        {
            this.СonfigurationPageSelected = obj.ConfigurationPages?.Count > 0 ? obj.ConfigurationPages[0] : null;
        }
        #endregion
        #region prorepty СonfigurationPageSelected
        private ConfigurationPage _сonfigurationPageSelected;
        public ConfigurationPage СonfigurationPageSelected { get => _сonfigurationPageSelected; set => 
                SetValue(nameof(СonfigurationPageSelected), ref _сonfigurationPageSelected, value, СonfigurationPageSelectedChanged); }

        private void СonfigurationPageSelectedChanged(ConfigurationPage obj)
        {
            
        }
        #endregion

        #region prorepty СonfigurationPageSelected
   
        public ObservableCollection<ConfigurationPage> ConfigurationPagesList
        {
            get => _settingsController?.ConfigurationPages != null ? new ObservableCollection<ConfigurationPage>( _settingsController.ConfigurationPages) : null;
            
        }


        #endregion
        #region prorepty ValidationResults

        public BindingList<SettingsError> ValidationResults
        {
            get => _settingsController.ValidationErrors;

        }


        #endregion
        #region prorepty ShowValidationResults
        private bool _showValidationResults;
        public  bool ShowValidationResults
        {
            get => _showValidationResults;

            set => this.SetValue("ShowValidationResults", ref _showValidationResults, value, ShowValidationResultsChanged);
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
        private SettingsError _validationResultSelected;
        public SettingsError ValidationResultSelected
        {
            get => _validationResultSelected;
            set => this.SetValue(nameof(ValidationResultSelected), ref _validationResultSelected, value, ValidationResultsSelectedChanged);

        }
        private void ValidationResultsSelectedChanged(SettingsError obj)
        {
      /*      if(obj != null)
            {
                СonfigurationPageSelected = null;
                СonfigurationPageSelected = _settingsController.ConfigurationPages.FirstOrDefault(x => x.GetType() == obj.SectionType);
             
            }*/
        }
        #endregion

        public SettingsDialogViewModel(SettingsDialogService settingsDialogService) : this(new SettingsController(), settingsDialogService) { }
        public SettingsDialogViewModel(SettingsController settingsController, SettingsDialogService settingsDialogService)
        {
            _log = LogManager.GetLogger(typeof(SettingsDialogViewModel));
            _settingsDialogService = settingsDialogService;
            _settingsController = settingsController;
            _settingsController.ValidationErrors.ListChanged += ValidationErrors_ListChanged; ;

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
            _settingsController.LoadAllPages();
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

            if (_settingsController.SaveAllSections()) 
                _settingsDialogService.CloseDialog();
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

            _settingsController.SaveAllSections(true);
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
            СonfigurationPageSelected = _settingsController.ConfigurationPages.FirstOrDefault(x => x.GetType() == ValidationResultSelected?.SectionType);
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
            _settingsDialogService.CloseDialog();
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
            _settingsController.HandleSettingsChanged(this.СonfigurationPageSelected);
            this.OnPropertyChanged("ConfigurationPagesList");


        }
        #endregion
        #endregion


    }
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this._execute(parameter);
        }
    }

}
