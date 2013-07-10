using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System.Collections.ObjectModel;
using System.Linq;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabsForHistory : TabsCollection
    {
        #region Constructors

        /// <summary>
        /// Selects tabs sorted by last time opened
        /// </summary>
        /// <param name="n">Number of tabs taken from the top</param>
        public TabsForHistory(int n)
        {
            IDataContextService database = Container.Resolve<IDataContextService>();

            Tabs = new ObservableCollection<TabEntity>((from Tab tab in database.Tabs
                    orderby tab.LastOpened descending
                    select tab).Take(n).Select(tab => tab.CreateEntity()));
            Initialize();
        }

        #endregion Constructors
    }
}
