using System;
using PhoneGuitarTab.Core.Navigation;
using PhoneGuitarTab.UI.Infrastructure.Enums;

namespace PhoneGuitarTab.UI.Infrastructure
{
    public class PageType : IPageType
    {
        private ViewType _type;

        public PageType(ViewType type)
        {
            _type = type;
        }

        public static PageType Get(string type)
        {
            return Get((ViewType)Enum.Parse(typeof(ViewType), type, true));
        }

        public static PageType Get(ViewType type)
        {
            return new PageType(type);
        }

        public bool Equals(IPageType other)
        {
            return _type.Equals((other as PageType)._type);
        } 
    }
}
