using System.IO.IsolatedStorage;

namespace PhoneGuitarTab.Core.Services
{
    public  class IsolatedStorageSettingService : ISettingService
    {
        private IsolatedStorageSettings _settings;

        public IsolatedStorageSettingService()
        {
            this._settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public void Save(string key, object value)
        {
            this._settings[key] = value;
            this._settings.Save();
        }
        public void Save()
        {
            _settings.Save();
        }
        public bool IsExist(string key)
        {
            return this._settings.Contains(key);
        }

        public T Load<T>(string key)
        {
            if (!this._settings.Contains(key))
                return default(T);

            return (T)this._settings[key];
        }
       
        public bool AddOrUpdateValue(string Key, object value)
        {

            bool valueChanged = false;

            // If the key exists
            if (_settings.Contains(Key))
            {
                // If the value has changed
                if (_settings[Key] != value)
                {
                    // Store the new value
                    _settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                _settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
            
        }

        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (_settings.Contains(Key))
            {
                value = (T)_settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }
    }
}
