using PhoneGuitarTab.Core.Services;

namespace PhoneGuitarTab.UnitTests.Core.Proxies
{
    using System.Reflection;

    using PhoneGuitarTab.Core.Dependencies.Interception.Proxies;

    public class SettingServiceProxy : ProxyBase, ISettingService
    {
        public void Save()
        {
            throw new System.NotImplementedException();
        }

        public void Save(System.String key, System.Object value)
        {
            var methodInvocation = this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), key, value);
            this.RunBehaviors(methodInvocation);
        }

        public bool AddOrUpdateValue(string Key, object value)
        {
            throw new System.NotImplementedException();
        }

        public System.Boolean IsExist(System.String key)
        {
            var methodInvocation = this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), key);
            return this.RunBehaviors(methodInvocation).GetReturnValue<System.Boolean>();
        }

        public T Load<T>(System.String key)
        {
            var methodInvocation = this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), key);
            methodInvocation.GenericTypes.Add(typeof(T));
            return this.RunBehaviors(methodInvocation).GetReturnValue<T>();
        }

        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            throw new System.NotImplementedException();
        }
    }
}
