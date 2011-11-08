using System;
using PhoneGuitarTab.Core.Navigation;

namespace PhoneGuitarTab.UI.Infrastructure
{
    public class PageType : IPageType
    {
        private EnumType _type;

        public PageType(EnumType type)
        {
            _type = type;
        }

        public static PageType Get(string type)
        {
            return Get((EnumType)Enum.Parse(typeof(EnumType), type, true));
        }

        public static PageType Get(EnumType type)
        {
            return new PageType(type);
        }

        public bool Equals(IPageType other)
        {
            return _type.Equals((other as PageType)._type);
        }

        public enum EnumType
        {
            Startup,
            Collection,
            Group,
            Search,
            Settings,
            Tab,
            TextTab,
            Help
        }
    }
}
