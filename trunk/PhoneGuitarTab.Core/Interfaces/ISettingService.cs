namespace PhoneGuitarTab.Core
{
    public interface ISettingService
    {
        void Save(string key, object value);
        bool IsExist(string key);
        T Load<T>(string key);
    }
}
