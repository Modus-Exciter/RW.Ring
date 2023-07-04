using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Notung.Feuerzauber.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {

        public SettingsDialog()
        {
            InitializeComponent();



            this.Loaded += (s, e) => {
                if (this.DataContext is Configuration.SettingsDialogViewModel controller) controller?.LoadedCommand?.Execute(null);
            };
        }
        private void ContentControl_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            Debug.WriteLine(e.TargetObject);
        }


        private void ContentControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
