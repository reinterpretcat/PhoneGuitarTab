namespace PhoneGuitarTab.Data
{
    using System;
    using System.Data.Linq;
    using System.Linq;

    using PhoneGuitarTab.Data;

    /// <summary>
    /// Unit of works over data context
    /// </summary>
    public class DataContextService : IDataContextService, IDisposable
    {
        private readonly TabDataContext _database;

        public DataContextService(string connectionString, Action<IDataContextService> initialize)
        {
            this._database = new TabDataContext(connectionString);

            if (!this._database.DatabaseExists())
            {
                //create the local database
                this._database.CreateDatabase();
                initialize(new TabDataContextInitializator(this._database));
                this._database.SubmitChanges();
            }
        }


        /// <summary>
        /// It is used by external classes in order to provide startup initialization through Data Context instantiation
        /// </summary>
        private class TabDataContextInitializator : IDataContextService
        {
            private readonly TabDataContext _database;
            public TabDataContextInitializator(TabDataContext database)
            {
                this._database = database;
            }
            public Table<Tab> Tabs
            {
                get { return this._database.Tabs; }
            }

            public Table<TabType> TabTypes
            {
                get { return this._database.TabTypes; }
            }

            public Table<Group> Groups
            {
                get { return this._database.Groups; }
            }

            public void SubmitChanges()
            {
                this._database.SubmitChanges();
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
        }


        #region ITabDataContextService memnbers

        // Specify a table for the tabs.
        public Table<Tab> Tabs
        {
            get { return this._database.Tabs; }
        }

        // Specify a table for the tab types.
        public Table<TabType> TabTypes
        {
            get { return this._database.TabTypes; }
        }

        // Specify a table for the groups.
        public Table<Group> Groups
        {
            get { return this._database.Groups; }
        }

        public void SubmitChanges()
        {
            this._database.SubmitChanges();
            this.InvokeOnChanged(new EventArgs());
        }

        #endregion ITabDataContextService memnbers

        public event EventHandler<EventArgs> OnChanged;

        private void InvokeOnChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.OnChanged;
            if (handler != null) handler(this, e);
        }

        public void Dispose()
        {
            this._database.Dispose();
        }

        public void InsertTab(Tab tab)
        {
            this.Tabs.InsertOnSubmit(tab);
            this.SubmitChanges();
        }

        public void DeleteTabById(int id)
        {
            Tab tab = (from Tab t in this.Tabs
                       where t.Id == id
                       select t).Single();
            Group group = tab.Group;
            this.Tabs.DeleteOnSubmit(tab);
            if (group.Tabs.Count <= 1)
                this.Groups.DeleteOnSubmit(group);

            this.SubmitChanges();
        }

        public Group GetOrCreateGroupByName(string name)
        {
            Group group = (from Group g in this.Groups
                           where g.Name == name
                           select g).SingleOrDefault();
            if (group == null)
            {
                group = new Group() { Name = name, ImageUrl = "/Images/all/Group.png" };
                this.Groups.InsertOnSubmit(group);
                this.SubmitChanges();
                //NOTE: g should be tracked automatically
            }
            return group;
        }

        public TabType GetTabTypeByName(string name)
        {
            return (from TabType t in this.TabTypes
                    where t.Name == name
                    select t).Single();
        }
    }
}
