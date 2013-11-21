using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
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

        private TestContext _testContext;

        [TestInitialize]
        public void SetUp()
        {
            //_dataService = CreateDataContext();
            //_cloudService = CreateCloudService();

            _testContext = new TestContext();
        }

        [TestMethod]
        public void CanSynchronize()
        {
            // TODO
        }


        private IDataContextService CreateDataContext()
        {
            var dataMock = new Mock<IDataContextService>();

            dataMock.Setup(ds => ds.GetOrCreateGroupByName(It.IsAny<string>())).Returns((string s) =>
            {
                var groupMock = new Mock<Group>();
                groupMock.SetupGet(g => g.Tabs).Returns(() =>
                {
                    var tabs = new EntitySet<Tab>();
                    tabs.AddRange(Enumerable.Range(1, 3).Select(i =>
                    {
                        var tabMock = new Mock<Tab>();
                        tabMock.SetupGet(t => t.Id).Returns(i);
                        tabMock.SetupGet(t => t.Name).Returns(string.Format("Iso_{0}{1}.txt", s, i));
                        return tabMock.Object;
                    }));
                    return tabs;
                });

                return groupMock.Object;
            });

            dataMock.Setup(ds => ds.InsertTab(It.IsAny<Tab>())).Callback((Tab tab) => _testContext.IsoInsertedTabs.Add(tab));

            dataMock.SetupGet(ds => ds.TabTypes).Returns(() =>
            {
                var tabTypes = new Mock<Table<TabType>>();
                tabTypes.Setup(t => t.Single()).Returns(() =>
                {
                    var tabTypeMock = new Mock<TabType>();
                    tabTypeMock.SetupGet(tt => tt.Name).Returns(() => "tab");
                    return tabTypeMock.Object;
                });

                return tabTypes.Object;
            });

            return dataMock.Object;
        }

        private ICloudService CreateCloudService()
        {
            var cloudMock = new Mock<ICloudService>();

            cloudMock.Setup(c => c.CreateDirectory(It.IsAny<string>()))
                .Callback((string path) => _testContext.CloudCreatedPath.Add(path));

            cloudMock.Setup(c => c.DeleteFile(It.IsAny<string>()))
                .Callback((string path) => _testContext.CloudDeletedFile.Add(path));

            cloudMock.Setup(c => c.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
               .Callback((string p1,string p2) => _testContext.DownloadedFiles.Add(p1,p2));

            //called once on root
            cloudMock.Setup(c => c.GetDirectoryNames(It.IsAny<string>()))
               .Returns((string path) => GetTask<IEnumerable<string>>(new []
               {
                   "Opeth","Metallica"
               }));

            cloudMock.Setup(c => c.GetFileNames(It.IsAny<string>()))
              .Returns((string path) => GetTask(Enumerable.Range(1, 4).Select(i => string.Format("Cloud_{0}", path, i))));

            cloudMock.Setup(c => c.SynchronizeFile(It.IsAny<string>(), It.IsAny<string>()))
              .Callback((string path) => _testContext.CloudCreatedPath.Add(path));

            //cloudMock.Setup(c => c.UploadFile(It.IsAny<string>(), It.IsAny<string>()))
            // .Callback((string path) => _testContext.CloudCreatedPath.Add(path));

            return cloudMock.Object;
        }


        private Task<T> GetTask<T>(T result)
        {
            var task=new TaskCompletionSource<T>();
            task.SetResult(result);
            return task.Task;
        }

        private class TestContext
        {
            public List<string> CloudCreatedPath { get; set; }
            public List<string> CloudDeletedFile { get; set; } 
            public List<Tab> IsoInsertedTabs { get; set; }

            public Dictionary<string, string> DownloadedFiles { get; set; }

            public TestContext()
            {
                IsoInsertedTabs = new List<Tab>();
                CloudCreatedPath = new List<string>();
                CloudDeletedFile = new List<string>();
            }

        }
    }
}
