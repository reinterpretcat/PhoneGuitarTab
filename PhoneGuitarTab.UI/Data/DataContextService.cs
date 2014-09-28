using System;
using System.Data.Linq;
using System.Linq;
using System.Net;


namespace PhoneGuitarTab.UI.Data
{
    /// <summary>
    ///     Unit of works over data context
    /// </summary>
    public class DataContextService : IDataContextService, IDisposable
    {
        private readonly TabDataContext _database;

        private Group currentGroup;
        private Tab currentTab;

        public DataContextService(string connectionString, Action<IDataContextService> initialize)
        {
            _database = new TabDataContext(connectionString);

            if (!_database.DatabaseExists())
            {
                //create the local database
                _database.CreateDatabase();
                initialize(new TabDataContextInitializator(_database));
                _database.SubmitChanges();
            }
        }


        /// <summary>
        ///     It is used by external classes in order to provide startup initialization through Data Context instantiation
        /// </summary>
        private class TabDataContextInitializator : IDataContextService
        {
            private readonly TabDataContext _database;

            public TabDataContextInitializator(TabDataContext database)
            {
                _database = database;
            }

            public ITable<Tab> Tabs
            {
                get { return _database.Tabs; }
            }

            public ITable<TabType> TabTypes
            {
                get { return _database.TabTypes; }
            }

            public ITable<Group> Groups
            {
                get { return _database.Groups; }
            }

            public void SubmitChanges()
            {
                _database.SubmitChanges();
            }

            public event EventHandler<EventArgs> OnChanged;

            public void InsertTab(Tab tab)
            {
                throw new NotImplementedException();
            }

            public void DeleteTabById(int id)
            {
                throw new NotImplementedException();
            }

            public Group GetOrCreateGroupByName(string name)
            {
                throw new NotImplementedException();
            }

            public TabType GetTabTypeByName(string name)
            {
                throw new NotImplementedException();
            }

            public Tab GetTabById(int id)
            {
                throw new NotImplementedException();
            }

            public void UpdateTabMediaById(int tabId, string albumCover)
            {
                throw new NotImplementedException();
            }

            public void UpdateGroupMediaByName(string name, string normal, string large, string extraLarge)
            {
                throw new NotImplementedException();
            }


            public bool IsGroupExists(string name)
            {
                throw new NotImplementedException();
            }
        }

        #region ITabDataContextService members

        // Specify a table for the tabs.
        public ITable<Tab> Tabs
        {
            get { return _database.Tabs; }
        }

        // Specify a table for the tab types.
        public ITable<TabType> TabTypes
        {
            get { return _database.TabTypes; }
        }

        // Specify a table for the groups.
        public ITable<Group> Groups
        {
            get { return _database.Groups; }
        }

        public void SubmitChanges()
        {
            _database.SubmitChanges();
            InvokeOnChanged(new EventArgs());
        }

        #endregion ITabDataContextService memnbers

        public event EventHandler<EventArgs> OnChanged;

        private void InvokeOnChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = OnChanged;
            if (handler != null) handler(this, e);
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        public void InsertTab(Tab tab)
        {
            currentTab = tab;
            Tabs.InsertOnSubmit(tab);
            SubmitChanges();
           
        }

        public void DeleteTabById(int id)
        {
            Tab tab = (from Tab t in Tabs
                where t.Id == id
                select t).Single();
            Group group = tab.Group;
            Tabs.DeleteOnSubmit(tab);
            if (group.Tabs.Count <= 1)
                Groups.DeleteOnSubmit(group);

            SubmitChanges();
        }

        public void UpdateGroupMediaByName(string name, string normal, string large, string extraLarge)
        {
            Group group = (from Group g in Groups
                           where g.Name == name
                           select g).SingleOrDefault();
            if (group != null)
            {
                group.ImageUrl = normal;
                group.LargeImageUrl = large;
                group.ExtraLargeImageUrl = extraLarge;
               
                SubmitChanges();
            }
        }

        public void UpdateTabMediaById(int tabId, string albumCover)
        {
            var tab = this.GetTabById(tabId);
            if( tab!= null)
            { 
            tab.AlbumCoverImageUrl = albumCover;
            SubmitChanges();
            }
        }

        public Group GetOrCreateGroupByName(string name)
        {
            string defaultGroupImageUrl = "/Images/light/band_light.png";

            Group group = (from Group g in Groups
                where g.Name == name
                select g).SingleOrDefault();
            if (group == null)
            {
                group = new Group
                {
                    Name = name,
                    ImageUrl = defaultGroupImageUrl,
                    LargeImageUrl = "",
                    ExtraLargeImageUrl = ""
                };
                currentGroup = group;
                Groups.InsertOnSubmit(group);
                SubmitChanges();

               
                //NOTE: g should be tracked automatically
            }
            else if (String.IsNullOrEmpty(group.ImageUrl))
            {
                group.ImageUrl = defaultGroupImageUrl;
            }
            return group;
        }

        public TabType GetTabTypeByName(string name)
        {
            return (from TabType t in TabTypes
                where t.Name == name
                select t).Single();
        }

        public Tab GetTabById(int id)
        {
            return (from Tab t in Tabs
                where t.Id == id
                select t).Single();
        }

        public bool IsGroupExists(string name)
        {
            Group group = (from Group g in Groups
                           where g.Name == name
                           select g).SingleOrDefault();

            if (group == null)
                return false;
            return true;
        }
       
    }
}