namespace PhoneGuitarTab.Data
{
    using System;
    using System.Data.Linq;
    using System.Linq;
    using PhoneGuitarTab.Search.Lastfm;
    using PhoneGuitarTab.Data;
    using System.Net;

    /// <summary>
    /// Unit of works over data context
    /// </summary>
    public class DataContextService : IDataContextService, IDisposable
    {
        private readonly TabDataContext _database;

        private Group currentGroup;
        private Tab currentTab;
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
            public ITable<Tab> Tabs
            {
                get { return this._database.Tabs; }
            }

            public ITable<TabType> TabTypes
            {
                get { return this._database.TabTypes; }
            }

            public ITable<Group> Groups
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

            public Tab GetTabById(int id)
            {
                throw new NotImplementedException();
            }
        }


        #region ITabDataContextService members

        // Specify a table for the tabs.
        public ITable<Tab> Tabs
        {
            get { return this._database.Tabs; }
        }

        // Specify a table for the tab types.
        public ITable<TabType> TabTypes
        {
            get { return this._database.TabTypes; }
        }

        // Specify a table for the groups.
        public ITable<Group> Groups
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
            this.currentTab = tab;
            this.Tabs.InsertOnSubmit(tab);
            this.SubmitChanges();
            this.GetTabCover(this.currentTab);
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
            string defaultGroupImageUrl = "/Images/light/band_light.png";

            Group group = (from Group g in this.Groups
                           where g.Name == name
                           select g).SingleOrDefault();
            if (group == null)
            {
                group = new Group() { Name = name, ImageUrl = defaultGroupImageUrl, LargeImageUrl = "", ExtraLargeImageUrl = "" };
                this.currentGroup = group;
                this.Groups.InsertOnSubmit(group);
                this.SubmitChanges();

                this.GetImageUrlOnline(this.currentGroup);
                //NOTE: g should be tracked automatically
            }
            else if (String.IsNullOrEmpty(group.ImageUrl))
            {
                group.ImageUrl = defaultGroupImageUrl;
            }
            return group;
        }

        private void GetTabCover(Tab tab)
        {
            LastFmSearch tabSearch = new LastFmSearch(tab.Group.Name, tab.Name);
            tabSearch.SearchCompleted += TabAlbumSearchCompleted;
            tabSearch.Run();
        }

        private void GetImageUrlOnline(Group band)
        {
            LastFmSearch result = new LastFmSearch(band.Name);
            result.SearchCompleted += SearchCompleted;

            result.Run();
        }

        private void TabAlbumSearchCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            LastFmSearch result = sender as LastFmSearch;

            try
            {
                var albumCover = result.ImageUrl;

                if (!string.IsNullOrEmpty(albumCover))
                {
                    currentTab.AlbumCoverImageUrl = result.LargeImageUrl;
                    this.SubmitChanges();
                }
                else
                {
                    currentTab.AlbumCoverImageUrl = "";
                    //handle other conditions
                }
            }
            catch
            {

                //handle catch
            }


        }

        private void SearchCompleted(object sender, DownloadStringCompletedEventArgs e) 
        {
            LastFmSearch result = sender as LastFmSearch;

            currentGroup.ImageUrl = result.ImageUrl;
            currentGroup.LargeImageUrl = result.LargeImageUrl;
            currentGroup.ExtraLargeImageUrl = result.ExtraLargeImageUrl;
            this.SubmitChanges();
        }

        public TabType GetTabTypeByName(string name)
        {
            return (from TabType t in this.TabTypes
                    where t.Name == name
                    select t).Single();
        }

        public Tab GetTabById(int id) 
        {
            return (from Tab t in this.Tabs
                    where t.Id == id
                    select t).Single();
        }


    }
}
