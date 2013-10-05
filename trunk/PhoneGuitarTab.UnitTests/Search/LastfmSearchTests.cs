namespace PhoneGuitarTab.UnitTests.Search
{
    using System.Threading;

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using PhoneGuitarTab.Search.Lastfm;

    [TestClass]
    public class LastfmSearchTests
    {
        [TestMethod]
        public void CanSearchInfo()
        {
            var r = new SearchInfoResult(SearchContext.DefaulGroupName);
            var mre = new ManualResetEvent(false);
            r.SearchCompleted += (sender, args) => mre.Set();
            r.Run();

            mre.WaitOne(SearchContext.SearchTimeout);

            Assert.IsFalse(string.IsNullOrEmpty(r.Url));
            Assert.IsFalse(string.IsNullOrEmpty(r.Summary));
            Assert.IsFalse(string.IsNullOrEmpty(r.ImageUrl));

            Assert.IsTrue(r.Summary.ToUpper().Contains(SearchContext.DefaulGroupName.ToUpper()));
        }
    }
}
