using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System.Linq;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabsForBand : TabsCollection
    {
        #region Constructors

        public TabsForBand(int groupId)
        {
            IDataContextService database = Container.Resolve<IDataContextService>();
            
            Tabs = (from Tab tab in database.Tabs
                       orderby tab.Name
                       select tab).Where(tab => tab.GroupId == groupId).Select(tab => tab.CreateEntity()).ToList();

            Initialize();
        }

        #endregion Constructors
    }
}
