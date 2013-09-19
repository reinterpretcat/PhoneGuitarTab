using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PhoneGuitarTab.Tablatures.Helpers
{
    public static class JsonExtensions
    {
        public static T SafeValue<T>(this JObject json)
        {
            return json == null? default(T): json.Value<T>();
        }

        public static T SafeValue<T>(this JToken json)
        {
            return json == null ? default(T) : json.Value<T>();
        }

        public static T SafeValue<T>(this JToken json, T @default)
        {
            return json == null ? @default : json.Value<T>();
        }
    }
}
