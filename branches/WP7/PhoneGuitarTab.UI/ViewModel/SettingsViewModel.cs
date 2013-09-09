using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class SettingsViewModel
    {
        IsolatedStorageSettings settings;

        public SettingsViewModel()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }
    }
}
