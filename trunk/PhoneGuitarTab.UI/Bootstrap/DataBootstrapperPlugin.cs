namespace PhoneGuitarTab.UI.Bootstrap
{
    using System;
    using System.Linq;
    using Microsoft.Phone.Data.Linq;
    using PhoneGuitarTab.Core.Bootstrap;
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Data;

    /// <summary>
    /// Processes data-specific staff
    /// </summary>
    public class DataBootstrapperPlugin : IBootstrapperPlugin
    {
        private const int DbVersion = 3;
        const string DbConnectionString = "Data Source=isostore:/TabData.sdf";

        public string Name { get { return "Data"; } }

        [Dependency]
        private IContainer Container { get; set; }

        public bool Run()
        {
            Action<IDataContextService> initialzeDatabase = service =>
                {
                    // TODO change pictures
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = Strings.MusicXml, ImageUrl = "/Images/instrument/MusicXML", TileUrl="/View/TextTabView.xaml?" });
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = Strings.GuitarPro, ImageUrl = "/Images/instrument/Guitarpro", TileUrl="/View/StaveTabView.xaml?" });
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "tab", ImageUrl = "/Images/instrument/Electric-Guitar", TileUrl = "/View/TextTabView.xaml?" });
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "bass", ImageUrl = "/Images/instrument/Bass", TileUrl = "/View/TextTabView.xaml?" });
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "chords", ImageUrl = "/Images/instrument/Chords", TileUrl = "/View/TextTabView.xaml?" });
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "drums", ImageUrl = "/Images/instrument/Drums", TileUrl = "/View/TextTabView.xaml?" });

                    //TODO update existing Band Pictures
                };
            
            Container.Register(Component.For<IDataContextService>().Use<DataContextService>(DbConnectionString, initialzeDatabase).Singleton());
           
            var dbService = Container.Resolve<IDataContextService>();

            CheckDatabaseVesion(dbService);
            return true;
        }

        private void CheckDatabaseVesion(IDataContextService dbService)
        {
            TabDataContext dataContext = new TabDataContext(DbConnectionString);
            DatabaseSchemaUpdater dbUpdater = dataContext.CreateDatabaseSchemaUpdater();

            if (dbUpdater.DatabaseSchemaVersion < DbVersion)
                UpdateDataBase(dbService, dbUpdater);
        }

        private void UpdateDataBase(IDataContextService dbService, DatabaseSchemaUpdater dbUpdater)
        {
             // Db schema changes

            // Release 1.1
             if (!dbService.TabTypes.Any(type => type.Name == "chords"))
                 dbService.TabTypes.InsertOnSubmit(new TabType() { Name = "chords", ImageUrl = "/Images/instrument/Chords" });

             if (!dbService.TabTypes.Any(type => type.Name == "drums"))
                 dbService.TabTypes.InsertOnSubmit(new TabType() { Name = "drums", ImageUrl = "/Images/instrument/Drums" });

            // Release 2.0
             if (!dbService.TabTypes.Any(type => type.Name == Strings.GuitarPro))
                 dbService.TabTypes.InsertOnSubmit(new TabType() { Name = Strings.GuitarPro, ImageUrl = "/Images/instrument/Guitarpro" });

            // Release 3.0
            if (dbUpdater.DatabaseSchemaVersion > 0 && dbUpdater.DatabaseSchemaVersion < 3)
            {
                dbUpdater.AddColumn<Tab>("CloudName");
                dbUpdater.AddColumn<Tab>("AlbumCoverImageUrl");
      
                dbUpdater.AddColumn<Group>("LargeImageUrl");
                dbUpdater.AddColumn<Group>("ExtraLargeImageUrl");

                dbUpdater.AddColumn<TabType>("TileUrl");
                dbService.TabTypes.InsertOnSubmit(new TabType() { Name = Strings.MusicXml, ImageUrl = "/Images/instrument/MusicXML" });
               
            }

             dbService.SubmitChanges();

             // Add the new database version.
             dbUpdater.DatabaseSchemaVersion = DbVersion;

            // Perform the database update in a single transaction.
            dbUpdater.Execute();
        }


        public bool Update()
        {
            return true;
        }
    }
}
