using System.Collections.ObjectModel;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabInGroup : ObservableCollection<TabEntity>
    {
        public TabInGroup(string category)
        {
            Key = category;
        }

        public string Key { get; set; }

        public bool HasItems
        {
            get { return Count > 0; }
        }
    }
}