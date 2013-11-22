using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Moq;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Diagnostic;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UnitTests.Cloud
{
    [TestClass]
    public class SkyDriveServiceTests
    {
        private TestContext _testContext;

        private IEnumerable<string> _groups;
    
            
        [TestInitialize]
        public void SetUp()
        {
            _testContext = new TestContext();
            _groups = new[] {"Opeth", "Metallica"};
        }

        [TestMethod]
        public void CanSynchronize()
        {
            // assign
            var syncService = CreateTabSyncService();

            // act
            ManualResetEvent mre = new ManualResetEvent(false);
            syncService.Complete += (sender, args) => mre.Set();
            syncService.Synchronize();
            mre.WaitOne();

            // assert

            // TODO
        }

        private TabSyncService CreateTabSyncService()
        {
           return (new Container())
            .RegisterInstance(CreateDataContext())
            .RegisterInstance(CreateCloudService())
            .RegisterInstance(CreateTabFileStorage())
            .RegisterInstance(CreateFileSystemService())
            .RegisterInstance<ITrace>(new DefaultTrace())
            .Register(Component.For<TabSyncService>().Use<TabSyncService>())
            .Resolve<TabSyncService>();
        }


        private IDataContextService CreateDataContext()
        {
            var dataMock = new Mock<IDataContextService>();

            // creates mock for group in iso
            Func<string, Group> createGroupMock = s =>
            {
                var groupMock = new Mock<Group>();
                groupMock.SetupGet(g => g.Name).Returns(() => s);
                groupMock.SetupGet(g => g.Tabs).Returns(() =>
                {
                    var tabs = new EntitySet<Tab>();
                    if(_groups.Contains(s))
                        tabs.AddRange(Enumerable.Range(1, 3).Select(i =>
                        {
                            var nameTemplate = string.Format("Iso_{0}{1}", s, i);
                            var tabMock = new Mock<Tab>();
                            tabMock.SetupGet(t => t.Id).Returns(i);
                            tabMock.SetupGet(t => t.Name).Returns(nameTemplate);
                            tabMock.SetupGet(t => t.Path).Returns(nameTemplate + ".txt");
                            tabMock.SetupGet(t => t.Group).Returns(groupMock.Object);
                            return tabMock.Object;
                        }));
                        return tabs;
                });

                return groupMock.Object;
            };

            dataMock.Setup(ds => ds.GetOrCreateGroupByName(It.IsAny<string>()))
                .Returns(createGroupMock);

            dataMock.SetupGet(ds => ds.Groups).Returns(() =>
            {
                var groupsMock = new Mock<ITable<Group>>();
                groupsMock.Setup(g => g.GetEnumerator())
                    .Returns(() => _groups.Select(createGroupMock).GetEnumerator());
                return groupsMock.Object;
            });

            dataMock.Setup(ds => ds.InsertTab(It.IsAny<Tab>()))
                .Callback((Tab tab) => _testContext.IsoInsertedTabs.Add(tab));

            
            // tab types
            var tabTypeMock = new Mock<TabType>();
            tabTypeMock.SetupGet(tt => tt.Name).Returns(() => "guitar pro");
            var queryable = (new List<TabType> {tabTypeMock.Object}).AsQueryable();

            var tabTypes = new Mock<ITable<TabType>>();
            tabTypes.Setup(t => t.Provider).Returns(() => queryable.Provider);
            tabTypes.Setup(t => t.Expression).Returns(() => queryable.Expression);

            dataMock.SetupGet(ds => ds.TabTypes).Returns(() => tabTypes.Object);

            return dataMock.Object;
        }

        private ICloudService CreateCloudService()
        {
            var cloudMock = new Mock<ICloudService>();

            cloudMock.Setup(c => c.CreateDirectory(It.IsAny<string>()))
                .Returns((string path) =>
                {
                    _testContext.CloudCreatedPath.Add(path);
                    return GetTask(OperationStatus.Completed);
                });

            cloudMock.Setup(c => c.DeleteFile(It.IsAny<string>()))
                .Returns((string path) =>
                {
                    _testContext.CloudDeletedFile.Add(path);
                     return GetTask(OperationStatus.Completed);
                });

            cloudMock.Setup(c => c.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
               .Returns((string p1, string p2) =>
               {
                   _testContext.DownloadedFiles.Add(p1, p2);
                   return GetTask(OperationStatus.Completed);
               });

            //called once on root
            cloudMock.Setup(c => c.GetDirectoryNames(It.IsAny<string>()))
               .Returns((string path) => GetTask(_groups));

            cloudMock.Setup(c => c.GetFileNames(It.IsAny<string>()))
              .Returns((string path) =>
              {
                  // new tabs
                  var list = Enumerable.Range(1, 4).Select(i => string.Format("Cloud_{0}{1}.gp5", path, i)).ToList();

                  // deleted tabs (there is the following convention to store files in cloud: <name>_<id>.<extension>)
                  // where name - tab.Name, id - tab.Id from iso
                  list.Add("deleted_1.gp5");
                  list.Add("deleted_2.gp5");
                  return GetTask(list.AsEnumerable());
              });

            cloudMock.Setup(c => c.SynchronizeFile(It.IsAny<string>(), It.IsAny<string>()))
              .Returns((string p1, string p2) =>
              {
                  _testContext.SynchronizedFiles.Add(p1, p2);
                  return GetTask(OperationStatus.Completed);
              });

            return cloudMock.Object;
        }

        private TabFileStorage CreateTabFileStorage()
        {
            var storageMock = new Mock<TabFileStorage>();
            storageMock.Setup(s => s.CreateTabFilePath()).Returns(() => Guid.NewGuid().ToString());
            // TODO
            return storageMock.Object;
        }

        private IFileSystemService CreateFileSystemService()
        {
            var fileSystemMock = new Mock<IFileSystemService>();

            return fileSystemMock.Object;
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
            public Dictionary<string, string> SynchronizedFiles { get; set; }

            public TestContext()
            {
                IsoInsertedTabs = new List<Tab>();
                CloudCreatedPath = new List<string>();
                CloudDeletedFile = new List<string>();
                DownloadedFiles = new Dictionary<string, string>();
                SynchronizedFiles = new Dictionary<string, string>();
            }

        }
    }
}
