using System;
using PhoneGuitarTab.Data;

namespace PhoneGuitarTab.UI.Entities
{
    public static class ControlExtentions
    {

        public static TabEntity CreateEntity(this Tab tab)
        {
            return new TabEntity()
                       {
                           Id = tab.Id,
                           Name = tab.Name,
                           Type = tab.TabType.Name,
                           ImageUrl = tab.TabType.ImageUrl,
                           Path = tab.Path,
                           Description = tab.Description,
                           Group = tab.Group.Name,
                           Rating = tab.Rating,
                           LastOpened = tab.LastOpened
                       };
        }

        public static string GetNameKey(this TabEntity tab)
        {
            char key = char.ToLower(tab.Name[0]);

            if (key < 'a' || key > 'z')
                key = '#';

            return key.ToString();
        }

        public static string GetNameKey(this Group group)
        {
            char key = char.ToLower(group.Name[0]);

            if (key < 'a' || key > 'z')
                key = '#';

            return key.ToString();
        }


    }
}
