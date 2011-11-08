using System;
using System.Data.Linq;

namespace PhoneGuitarTab.Data
{
    public class DataContextService : IDataContextService
    {
        private readonly TabDataContext _database;

        public DataContextService(string connectionString, Action<IDataContextService> initialize)
        {
            _database = new TabDataContext(connectionString);

            if (!_database.DatabaseExists())
            {
                //create the local database
                _database.CreateDatabase();
                initialize(new TabDataContextInitializator(_database));
                //InitializeDatabase();
                _database.SubmitChanges();
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
                _database = database;
            }
            public Table<Tab> Tabs
            {
                get { return _database.Tabs; }
            }

            public Table<TabType> TabTypes
            {
                get { return _database.TabTypes; }
            }

            public Table<Group> Groups
            {
                get { return _database.Groups; }
            }

            public void SubmitChanges()
            {
                _database.SubmitChanges();
            }


            public event EventHandler<EventArgs> OnChanged;
        }
        #region ITabDataContextService

        // Specify a table for the tabs.
        public Table<Tab> Tabs
        {
            get { return _database.Tabs; }
        }

        // Specify a table for the tab types.
        public Table<TabType> TabTypes
        {
            get { return _database.TabTypes; }
        }

        // Specify a table for the groups.
        public Table<Group> Groups
        {
            get { return _database.Groups; }
        }

        public void SubmitChanges()
        {
            _database.SubmitChanges();
            InvokeOnChanged(new EventArgs());
        }

        #endregion



        public event EventHandler<EventArgs> OnChanged;

        private void InvokeOnChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = OnChanged;
            if (handler != null) handler(this, e);
        }
    }
}
