namespace PhoneGuitarTab.Core.Services
{
    public interface ISettingService
    {
        void Save();
        void Save(string key, object value);
        bool AddOrUpdateValue(string Key, object value);
        bool IsExist(string key);
        T Load<T>(string key);
        T GetValueOrDefault<T>(string Key, T defaultValue);
    }
}