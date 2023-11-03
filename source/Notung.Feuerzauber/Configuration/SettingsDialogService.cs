using Notung.Feuerzauber.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.Feuerzauber.Configuration
{
    public class SettingsDialogService
    {
        private SettingsDialog dialogWindow;

        public void ShowDialog()
        {
            dialogWindow = new SettingsDialog() { DataContext = new SettingsDialogViewModel(this) };
            dialogWindow.ShowDialog();
        }

        public void CloseDialog()
        {
            if (dialogWindow != null)
            {
                dialogWindow.Close();
                dialogWindow.DataContext = null;
                dialogWindow = null;
            }
        }
    }
}
