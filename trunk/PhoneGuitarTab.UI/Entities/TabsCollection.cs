using System.Collections.Generic;

namespace PhoneGuitarTab.UI.Entities
{
    public abstract class TabsCollection : List<TabInGroup>
    {
        #region Fields

        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";

        #endregion Fields


        #region Properties

        public List<TabEntity> Tabs { get; protected set; }

        #endregion Properties


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
    }
}
