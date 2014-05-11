using System.Collections.ObjectModel;
using System.Linq;
using PhoneGuitarTab.UI.Data;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabsForHistory : TabsGroupsCollection
    {
        #region Constructors

        /// <summary>
        ///     Selects tabs sorted by last time opened
        /// </summary>
        /// <param name="n">Number of tabs taken from the top</param>
        /// <param name="database"> </param>
        public TabsForHistory(int n, IDataContextService database) : base(database)
        {
            Tabs = new ObservableCollection<TabEntity>((from Tab tab in Database.Tabs
                orderby tab.LastOpened descending
                select tab).Where(t => t.LastOpened != null).Take(n).Select(tab => tab.CreateEntity()));
            Initialize();
        }

        #endregion Constructors
    }
}