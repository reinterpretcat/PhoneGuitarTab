using System.IO.IsolatedStorage;

namespace PhoneGuitarTab.Core.Services
{
    public class IsolatedStorageSettingService : ISettingService
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
    }
}
