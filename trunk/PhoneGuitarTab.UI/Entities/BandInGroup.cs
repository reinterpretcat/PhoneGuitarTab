using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System.Collections.ObjectModel;

namespace PhoneGuitarTab.UI.Entities
{
    public class BandInGroup : ObservableCollection<ObservableTuple<int, Group>>
    {
        public BandInGroup(string category)
        {
            Key = category;
        }

        public string Key { get; set; }

        public bool HasItems { get { return Count > 0; } }
    }
}
