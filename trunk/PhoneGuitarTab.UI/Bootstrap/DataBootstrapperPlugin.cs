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
        private const int DbVersion = 2;
        const string DbConnectionString = "Data Source=isostore:/TabData.sdf";

        public string Name { get { return "Data"; } }

        [Dependency]
        private IContainer Container { get; set; }

        public bool Run()
        {
            Action<IDataContextService> initialzeDatabase = service =>
                {
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "guitar pro", ImageUrl = "/Images/all/TabText.png" });
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "tab", ImageUrl = "/Images/all/TabText.png" });
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "bass", ImageUrl = "/Images/all/TabText.png" });
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "chords", ImageUrl = "/Images/all/TabText.png" });
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "drums", ImageUrl = "/Images/all/TabText.png" });
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
          
            // Add the new database version.
             dbUpdater.DatabaseSchemaVersion = DbVersion;

             // - changes since db version 0 -
             if (!dbService.TabTypes.Any(type => type.Name == "chords"))
                 dbService.TabTypes.InsertOnSubmit(new TabType() { Name = "chords", ImageUrl = "/Images/all/TabText.png" });

             if (!dbService.TabTypes.Any(type => type.Name == "drums"))
                 dbService.TabTypes.InsertOnSubmit(new TabType() { Name = "drums", ImageUrl = "/Images/all/TabText.png" });

             if (!dbService.TabTypes.Any(type => type.Name == "guitar pro"))
                 dbService.TabTypes.InsertOnSubmit(new TabType() { Name = "guitar pro", ImageUrl = "/Images/all/TabText.png" });

             // --

             dbService.SubmitChanges();

             //here goes schema update if needed
             //...
             
            // Perform the database update in a single transaction.
            dbUpdater.Execute();
        }


        public bool Update()
        {
            return true;
        }
    }
}
