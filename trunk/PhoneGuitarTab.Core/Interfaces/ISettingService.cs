using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.Core
{
    public interface ISettingService
    {
        void Save(string key, object value);
        bool IsExist(string key);
        T Load<T>(string key);
    }
}
