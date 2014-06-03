
using PhoneGuitarTab.Search.Tabs;

namespace PhoneGuitarTab.UnitTests.Search
{
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Threading;

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using PhoneGuitarTab.Search;

    [TestClass]
    public class UltimateGuitarDownloadTests
    {
        [TestMethod]
        public void CanDownloadGuitarProTab()
        {
            DownloadTabByType(TabulatureType.Guitar);
        }

        [TestMethod]
        public void CanDownloadTextTab()
        {
            DownloadTabByType(TabulatureType.Guitar);
        }

        private void DownloadTabByType(TabulatureType type)
        {
            var destination = Guid.NewGuid().ToString();

            var searcher = new UltimateGuitarTabSearcher();
            var mre = new ManualResetEvent(false);
            searcher.SearchComplete += (sender, args) =>
            {
                var entry = searcher.Entries.First();

                var downloader = new UltimateGuitarFileDownloader(entry, destination);
                downloader.DownloadComplete += (o, eventArgs) => mre.Set();
                downloader.Download();

            };

            searcher.Run(SearchContext.DefaulGroupName, SearchContext.DefaultSongName, 0, type);

            mre.WaitOne(SearchContext.SearchTimeout);

            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var file = storage.OpenFile(destination, FileMode.Open))
                {
                    Assert.AreNotEqual(0, file.Length);
                }
            }
        }
    }
}
