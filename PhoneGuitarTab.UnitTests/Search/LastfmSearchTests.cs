using PhoneGuitarTab.Search.Arts;

namespace PhoneGuitarTab.UnitTests.Search
{
    using System.Threading;

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    [TestClass]
    public class LastfmSearchTests
    {
        [TestMethod]
        public void CanSearchInfo()
        {
            var r = new LastFmMediaSearcher();
            var mre = new ManualResetEvent(false);
            r.MediaSearchCompleted += (sender, args) => mre.Set();
            r.RunMediaSearch(SearchContext.DefaulGroupName, string.Empty);

            mre.WaitOne(SearchContext.SearchTimeout);

            Assert.IsFalse(string.IsNullOrEmpty(r.Entry.Url));
            Assert.IsFalse(string.IsNullOrEmpty(r.Entry.Summary));
            Assert.IsFalse(string.IsNullOrEmpty(r.Entry.ImageUrl));

            Assert.IsTrue(r.Entry.Summary.ToUpper().Contains(SearchContext.DefaulGroupName.ToUpper()));
        }
    }
}
