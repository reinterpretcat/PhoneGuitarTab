using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneGuitarTab.Core
{
    public class IsolatedStorageSettingService : ISettingService
    {
        private IsolatedStorageSettings _settings;
        public IsolatedStorageSettingService()
        {
            _settings = IsolatedStorageSettings.ApplicationSettings;

        }

        public void Save(string key, object value)
        {
            _settings[key] = value;
            _settings.Save();
        }

        public bool IsExist(string key)
        {
            return _settings.Contains(key);
        }

        public T Load<T>(string key)
        {
            if (!_settings.Contains(key))
                return default(T);

            return (T)_settings[key];
        }
    }
}
