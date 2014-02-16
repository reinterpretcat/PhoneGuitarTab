using PhoneGuitarTab.UI.Data;

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
        private const int DbVersion = 4;
        const string DbConnectionString = "Data Source=isostore:/TabData.sdf";

        public string Name { get { return "Data"; } }

        [Dependency]
        private IContainer Container { get; set; }

        public bool Run()
        {
            Action<IDataContextService> onCreateDb = service =>
                {
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = Strings.MusicXml, ImageUrl = "/Images/instrument/MusicXML"});
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = Strings.GuitarPro, ImageUrl = "/Images/instrument/Guitarpro"});
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "tab", ImageUrl = "/Images/instrument/Electric-Guitar"});
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "bass", ImageUrl = "/Images/instrument/Bass"});
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "chords", ImageUrl = "/Images/instrument/Chords"});
                    service.TabTypes.InsertOnSubmit(new TabType() { Name = "drums", ImageUrl = "/Images/instrument/Drums" });
                };

            Container.Register(Component.For<IDataContextService>().Use<DataContextService>(DbConnectionString, onCreateDb).Singleton());
           
            (new UpdateScript(Container.Resolve<IDataContextService>(), DbConnectionString, DbVersion))
                .CheckAndUpdate();
            
            return true;
        }



        public bool Update()
        {
            return true;
        }
    }
}
