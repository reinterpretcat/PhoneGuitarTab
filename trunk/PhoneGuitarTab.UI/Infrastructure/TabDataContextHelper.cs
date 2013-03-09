using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System.Linq;

namespace PhoneGuitarTab.UI.Infrastructure
{
    internal static class TabDataContextHelper
    {
        public static void InitializeDatabase(IDataContextService service)
        {
            //add initial values
            TabType t1 = new TabType() { Name = "tab pro", ImageUrl = "/Images/all/TabGP.png" };
            service.TabTypes.InsertOnSubmit(t1);
            TabType t2 = new TabType() { Name = "power tab", ImageUrl = "/Images/all/TabPTB.jpg" };
            service.TabTypes.InsertOnSubmit(t2);
            service.TabTypes.InsertOnSubmit(new TabType() { Name = "tab", ImageUrl = "/Images/all/TabText.png" });
            service.TabTypes.InsertOnSubmit(new TabType() { Name = "bass", ImageUrl = "/Images/all/TabText.png" });
        }

        public static void InsertTab(Tab tab)
        {
            var database = Container.Resolve<IDataContextService>();
            database.Tabs.InsertOnSubmit(tab);
            database.SubmitChanges();
        }

        public static void DeleteTabById(int id)
        {
            IDataContextService database = Container.Resolve<IDataContextService>();
            Tab tab = (from Tab t in database.Tabs
                       where t.Id == id
                       select t).Single();
            Group group = tab.Group;
            database.Tabs.DeleteOnSubmit(tab);
            if (group.Tabs.Count <= 1)
                database.Groups.DeleteOnSubmit(group);

            database.SubmitChanges();          
        }

        public static Group GetOrCreateGroupByName(string name)
        {
            IDataContextService database = Container.Resolve<IDataContextService>();
            Group group = (from Group g in database.Groups
                       where g.Name == name
                       select g).SingleOrDefault();
            if (group == null)
            {
                group = new Group() { Name = name, ImageUrl = "/Images/all/Group.png" };
                database.Groups.InsertOnSubmit(group);
                database.SubmitChanges();
                //NOTE: g should be tracked automatically
            }
            return group;
        }

        public static TabType GetTabTypeByName(string name)
        {
            IDataContextService database = Container.Resolve<IDataContextService>();
            return (from TabType t in database.TabTypes
                    where t.Name == name
                    select t).Single();
        }
    }
}
