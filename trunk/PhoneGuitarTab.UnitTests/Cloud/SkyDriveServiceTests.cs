using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core;
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
        private string _newGroup = "Summoning";
            
        [TestInitialize]
        public void SetUp()
        {
            _testContext = new TestContext();
            _groups = new[] {"Opeth", "Metallica"};
        }

        #region Test description
        /*
        Initial state:
	    Iso:
		    -Opeth
			    Iso_Opeth1
			    Iso_Opeth2
			    Iso_Opeth3
			    Cloud_old_Opeth // CloudName isn't null
		    -Metallica
			    Iso_Metallica1
			    Iso_Metallica2
			    Iso_Metallica3
			    Cloud_old_Metallica // CloudName isn't null
	    Cloud:
		    -Opeth
			    Cloud_new_Opeth1.gp5 // new
			    Cloud_new_Opeth2.gp5 // new
			    deleted_sync_1.gp5 // already synchronized, but deleted on phone
			    deleted_sync_2.gp5 // already synchronized, but deleted on phone
			    Iso_Opeth1_sync_1.gp5 // already synchronized; uploaded from phone
			    Cloud_old_Opeth.gp5 // already synchronized; downloaded to phone
		    -Metallica
			    Cloud_new_Metallica1.gp5 // new 
			    Cloud_new_Metallica2.gp5 // new
			    deleted_sync_1.gp5 // already synchronized, but deleted on phone
			    deleted_sync_2.gp5 // already synchronized, but deleted on phone
			    Iso_Metallica1_sync_1.gp5 // already synchronized; uploaded from phone
			    Cloud_old_Metallica.gp5 // already synchronized; downloaded to phone
            - Summoning
                Cloud_group_new_Summoning.gp5

        Expected state:
		        Iso:
		        -Opeth
			        Iso_Opeth1 //old
			        Iso_Opeth2 // old
			        Iso_Opeth3 // old
			        Cloud_old_Opeth // old
			        Cloud_new_Opeth1 //new
			        Cloud_new_Opeth2 //new
		        -Metallica
			        Iso_Metallica1 // old
			        Iso_Metallica2 // old
			        Iso_Metallica3 // old
			        Cloud_old_Metallica // old
			        Cloud_new_Metallica1.gp5 // new
			        Cloud_new_Metallica2.gp5 // new
                -Summoning
                    Cloud_group_new_Summoning.gp5 // new
			
	        Cloud:
		        -Opeth
			        Iso_Opeth1_sync_1.gp5 // new
			        Iso_Opeth2_sync_2.gp5 // new
			        Iso_Opeth3_sync_3.gp5 // new
			        Cloud_new_Opeth1.gp5 // old
			        Cloud_new_Opeth2.gp5 // old
			        Cloud_old_Opeth // old
			        Iso_Opeth1_sync_1.gp5 // synchronized
		        -Metallica
			        Iso_Metallica1_sync_1.gp5 // new
			        Iso_Metallica2_sync_2.gp5 // new
			        Iso_Metallica3_sync_3.gp5 // new
			        Cloud_new_Metallica1.gp5 // old
			        Cloud_new_Metallica2.gp5 // old
			        Cloud_old_Metallica // old
			        Iso_Metallica1_sync_1.gp5 // synchronized
                - Summoning
                    Cloud_group_new_Summoning.gp5
         * 
         * */
        #endregion

        [TestMethod]
        public void CanSynchronize()
        {

            // assign
            var syncService = CreateTabSyncService();

            // act

            Action syncAction = () =>
            {
                ManualResetEvent mre = new ManualResetEvent(false);
                syncService.Complete += (sender, args) => mre.Set();
                syncService.Synchronize();
                mre.WaitOne();
            };


            syncAction();
            // assert

            // Synchronized files
            Assert.AreEqual(8, _testContext.UploadedFiles.Count);
            _groups.ForEach(g =>
            {
                var nameTemplate = string.Format("Iso_{0}{1}", g, "{0}");
                var tabs = Enumerable.Range(1, 3).Select(i => string.Format(nameTemplate, i)).ToList();
                tabs.Add(string.Format("Cloud_old_{0}", g));
                var id = 1;
                tabs.ForEach(name =>
                {
                    var key = name + ".gp5";
                    Assert.IsTrue(_testContext.UploadedFiles.ContainsKey(key));

                    var suffix = name.Contains("old") ? "" : "_sync_" + id++;
                    var @value = string.Format("PhoneGuitarTab/{0}/{1}{2}.gp5", g, name, suffix);
                    Assert.AreEqual(@value, _testContext.UploadedFiles[key]);
                });
            });
           
            // TODO
            Assert.AreEqual(5, _testContext.IsoInsertedTabs.Distinct().Count());
            Assert.IsTrue(_testContext.IsoInsertedTabs.TrueForAll(t => t.Name.Contains("new")));
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

                    Func<string, int, Mock<Tab>> createTabMock = (template, i) =>
                    {
                        var tabMock = new Mock<Tab>();
                        tabMock.SetupGet(t => t.Id).Returns(i);
                        tabMock.SetupGet(t => t.TabType).Returns(() =>
                        {
                            var ttMock = new Mock<TabType>();
                            ttMock.SetupGet(tt => tt.Name).Returns(() => "guitar pro");
                            return ttMock.Object;
                        });
                        tabMock.SetupGet(t => t.Name).Returns(string.Format(template,i));
                        tabMock.SetupGet(t => t.Path).Returns(string.Format(template,i) + ".gp5");
                        tabMock.SetupGet(t => t.Group).Returns(groupMock.Object);
                        return tabMock;
                    };
                    if (_groups.Contains(s))
                    {
                        // regular tabs, should be synchronized
                        var nameTemplate = string.Format("Iso_{0}{1}", s, "{0}");
                        tabs.AddRange(Enumerable.Range(1, 3)
                            .Select(i => createTabMock(nameTemplate, i))
                            .Select(t => t.Object));

                        // sync before by downloading from cloud
                        var cloudName = string.Format("Cloud_old_{0}", s);
                        var tabMock = createTabMock(cloudName, 4);
                        tabMock.SetupGet(t => t.CloudName)
                            .Returns(string.Format("{0}.gp5", cloudName));
                            //.Returns(string.Format("PhoneGuitarTab/{0}/{1}.gp5", s, cloudName));
                        tabs.Add(tabMock.Object);
                    }

                    return tabs;
                });

                return groupMock.Object;
            };

            dataMock.Setup(ds => ds.GetOrCreateGroupByName(It.IsAny<string>()))
                .Returns(createGroupMock);

            dataMock.SetupGet(ds => ds.Groups).Returns(() =>
            {
                var groupsMock = new Mock<ITable<Group>>();
                var gQueryable = _groups.Select(createGroupMock).AsQueryable();

                groupsMock.Setup(t => t.Provider).Returns(() => gQueryable.Provider);
                groupsMock.Setup(t => t.Expression).Returns(() => gQueryable.Expression);
                groupsMock.Setup(g => g.GetEnumerator()).Returns(gQueryable.GetEnumerator);
                return groupsMock.Object;
            });



            dataMock.Setup(ds => ds.InsertTab(It.IsAny<Tab>()))
                .Callback((Tab tab) => _testContext.IsoInsertedTabs.Add(tab));

            
            // tab types
            var tabTypeMock = new Mock<TabType>();
            tabTypeMock.SetupGet(tt => tt.Name).Returns(() => "guitar pro");
            var ttQueryable = (new List<TabType> {tabTypeMock.Object}).AsQueryable();

            var tabTypes = new Mock<ITable<TabType>>();
            tabTypes.Setup(t => t.Provider).Returns(() => ttQueryable.Provider);
            tabTypes.Setup(t => t.Expression).Returns(() => ttQueryable.Expression);

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

            cloudMock.Setup(c => c.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
               .Returns((string p1, string p2) =>
               {
                   _testContext.DownloadedFiles.Add(p1, p2);
                   return GetTask(OperationStatus.Completed);
               });

            //called once on root
            cloudMock.Setup(c => c.GetDirectoryNames(It.IsAny<string>()))
               .Returns((string path) =>
               {
                   var list = new List<string>(_groups);
                   list.Add(_newGroup);
                   return GetTask(list.AsEnumerable());
               });

            cloudMock.Setup(c => c.GetFileNames(It.IsAny<string>()))
              .Returns((string path) =>
              {
                  var groupName = path.Split('/')[1];

                  // special case
                  if (groupName == _newGroup)
                      return GetTask(new[] { string.Format("{0}/Cloud_group_new_{1}.gp5", path, groupName) }.AsEnumerable());

                  // new tabs
                  var list = Enumerable.Range(1, 2).Select(i => string.Format("{0}/Cloud_new_{1}{2}.gp5", path, groupName, i)).ToList();

                  // deleted tabs (there is the following convention to store files in cloud: <name>_<id>.<extension>)
                  // where name - tab.Name, id - tab.Id from iso
                  list.Add(string.Format("{0}/deleted_sync_1.gp5", path));
                  list.Add(string.Format("{0}/deleted_sync_2.gp5", path));
                  list.Add(string.Format("{0}/Cloud_old_{1}.gp5", path, groupName));

                  list.Add(string.Format("{0}/Iso_{1}1_sync_1.gp5", path, groupName));

                  return GetTask(list.AsEnumerable());
              });

            cloudMock.Setup(c => c.FileExists(It.IsAny<string>()))
             .Returns((string p1) =>
             {
                 return GetTask(false);
             });

            cloudMock.Setup(c => c.UploadFile(It.IsAny<string>(), It.IsAny<string>()))
              .Returns((string p1, string p2) =>
              {
                  _testContext.UploadedFiles.Add(p1, p2);
                  return GetTask(OperationStatus.Completed);
              });

            cloudMock.Setup(c => c.Release()).Callback(() => { });

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
            public Dictionary<string, string> UploadedFiles { get; set; }

            public TestContext()
            {
                IsoInsertedTabs = new List<Tab>();
                CloudCreatedPath = new List<string>();
                CloudDeletedFile = new List<string>();
                DownloadedFiles = new Dictionary<string, string>();
                UploadedFiles = new Dictionary<string, string>();
            }

        }
    }
}
