using System.Collections.Generic;
using System.ComponentModel;

namespace PhoneGuitarTab.UI.Entities
{
    public abstract class TabsCollection : List<TabInGroup>, INotifyPropertyChanged
    {
        #region Fields

        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";

        #endregion Fields


        #region Properties

        public List<TabEntity> Tabs { get; protected set; }

        #endregion Properties


        #region Public methods

        public void RemoveTab(TabEntity tabToRemove)
        {
            if (tabToRemove != null)
            {
                Tabs.Remove(tabToRemove);
                RaisePropertyChanged("Tabs");
            }
        }

        #endregion Public methods


        #region Helper methods

        protected void Initialize()
        {
            Dictionary<string, TabInGroup> groups = new Dictionary<string, TabInGroup>();
            foreach (char c in Groups)
            {
                TabInGroup tab = new TabInGroup(c.ToString());
                this.Add(tab);
                groups[c.ToString()] = tab;
            }

            foreach (TabEntity t in Tabs)
            {
                groups[t.GetNameKey()].Add(t);
            }
        }

        #endregion Helper methods


        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        #endregion INotifyPropertyChanged members
    }
}
