
namespace PhoneGuitarTab.UnitTests.Search
{
    using System;
    using System.Threading;

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using PhoneGuitarTab.Search;
    using PhoneGuitarTab.Search.UltimateGuitar;

    [TestClass]
    public class UltimateGuitarSearchTests
    {
        [TestMethod]
        public void CanSearchForBand()
        {
            var searcher = new UltimateGuitarTabSearcher(SearchContext.DefaulGroupName, "");
            var mre = new ManualResetEvent(false);
            searcher.SearchComplete += (sender, args) => mre.Set();
            
            searcher.Run(0, TabulatureType.All);

            mre.WaitOne(SearchContext.SearchTimeout); // NOTE wait only two minutes

            Assert.AreEqual(searcher.Summary.BandsFound, 1);
            Assert.IsTrue(searcher.Summary.TotalSongsFound > 0);
            Assert.IsTrue(searcher.Summary.PageCount > 1);
        }

        [TestMethod]
        public void CanSearchForBandByPage()
        {
            var searcher = new UltimateGuitarTabSearcher(SearchContext.DefaulGroupName, "");
            var mre = new ManualResetEvent(false);
            searcher.SearchComplete += (sender, args) => mre.Set();

            searcher.Run(1, TabulatureType.All);

            mre.WaitOne(SearchContext.SearchTimeout);

            Assert.AreEqual(searcher.Summary.BandsFound, 1);
            Assert.IsTrue(searcher.Summary.TotalSongsFound > 0);
        }

        [TestMethod]
        public void CanSearchForSong()
        {
            var searcher = new UltimateGuitarTabSearcher(SearchContext.DefaulGroupName, SearchContext.DefaultSongName);
            var mre = new ManualResetEvent(false);
            searcher.SearchComplete += (sender, args) => mre.Set();
            
            searcher.Run(0, TabulatureType.All);

            mre.WaitOne(SearchContext.SearchTimeout); 

            Assert.AreEqual(searcher.Summary.BandsFound, 1);
            Assert.IsTrue(searcher.Summary.TotalSongsFound > 0);
        }
    }
}
