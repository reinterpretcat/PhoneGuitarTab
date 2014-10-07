using PhoneGuitarTab.Core.Bootstrap;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Diagnostic;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.Arts;
using PhoneGuitarTab.Search.Audio;
using PhoneGuitarTab.Search.Suggestions;
using PhoneGuitarTab.Search.Tabs;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.Bootstrap
{
    public class CoreBootstrapperPlugin : IBootstrapperPlugin
    {
        public string Name
        {
            get { return "Core"; }
        }

        [Dependency]
        private IContainer Container { get; set; }

        public bool Run()
        {
            Container.Register(Component.For<ITrace>().Use<DefaultTrace>().Singleton());
            Container.Register(Component.For<TraceCategory>().Use<TraceCategory>("Default").Singleton());
                // TODO create several named categories

            Container.Register(Component.For<IFileSystemService>().Use<IsolatedStorageFileService>().Singleton());
            Container.Register(Component.For<TabFileStorage>().Use<TabFileStorage>().Singleton());
            Container.Register(Component.For<ISettingService>().Use<IsolatedStorageSettingService>().Singleton());

            Container.Register(Component.For<MessageHub>().Use<MessageHub>().Singleton());

            Container.Register(Component.For<RatingService>().Use<RatingService>().Singleton());
            Container.Register(Component.For<IDialogController>().Use<ToastDialogController>().Singleton());

            Container.Register(Component.For<ITabSearcher>().Use<UltimateGuitarTabSearcher>());
            Container.Register(Component.For<IAudioSearcherFactory>().Use<AudioSearcherFactory>());
            Container.Register(Component.For<IMediaSearcherFactory>().Use<MediaSearcherFactory>());
            Container.Register(Component.For<IBandSuggestor>().Use<LastFmBandSuggestor>());
            Container.Register(Component.For<IGenreBrowser>().Use<LastFmGenreBrowser>());
           
            return true;
        }

        public bool Update()
        {
            return true;
        }
    }
}