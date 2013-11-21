using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Moq;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UnitTests.Cloud
{
    [TestClass]
    public class SkyDriveServiceTests
    {
        private ICloudService _cloudService;
        private IDataContextService _dataService;
        private IsolatedStorageFileService _isoService;
        private TabFileStorage _tabFileStorage;
        private TestContext _testContext;

        [TestInitialize]
        public void SetUp()
        {
            _dataService = CreateDataContext();
            _testContext = new TestContext();
        }

       // [TestMethod]
        public void CanSynchronize()
        {
            var group = _dataService.GetOrCreateGroupByName("Opeth");

            var tabs = group.Tabs.ToList();
        }


        private IDataContextService CreateDataContext()
        {
            var dataMock = new Mock<IDataContextService>();

            dataMock.Setup(ds => ds.GetOrCreateGroupByName(It.IsAny<string>())).Returns((string s) =>
            {
                var groupMock = new Mock<Group>();
                groupMock.Setup(g => g.Tabs).Returns(() =>
                {
                    var tabs = new EntitySet<Tab>();
                    tabs.AddRange(Enumerable.Range(1, 3).Select(i =>
                    {
                        var tabMock = new Mock<Tab>();
                        tabMock.Setup(t => t.Id).Returns(i);
                        tabMock.Setup(t => t.Name).Returns(string.Format("{0}.txt", s, i));
                        return tabMock.Object;
                    }));
                    return tabs;
                });

                return groupMock.Object;
            });

            dataMock.Setup(ds => ds.InsertTab(It.IsAny<Tab>())).Callback((Tab tab) => _testContext.IsoInsertedTabs.Add(tab));

            return dataMock.Object;
        }


        private class TestContext
        {
            public List<Tab> IsoInsertedTabs { get; set; }

            public TestContext()
            {
                IsoInsertedTabs = new List<Tab>();
                
            }

        }
    }
}
