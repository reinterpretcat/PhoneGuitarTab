using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PhoneGuitarTab.Core;

namespace PhoneGuitarTab.UI.Infrastructure
{
    public static class TabStorageHelper
    {
        private static readonly object SyncLock = new object();
        private static IFileSystemService _fileSystemService;
        public static IFileSystemService FileSystemService
        {
            get
            {
                if(_fileSystemService == null)
                {
                    lock (SyncLock)
                    {
                        if(_fileSystemService == null)
                        {
                            _fileSystemService = Container.Resolve<IFileSystemService>();
                        }
                    }
                }
                return _fileSystemService;
            }
        }

        private const string TabDirectory = "Tabs";

        public static string CreateTabFilePath()
        {
            if (!FileSystemService.DirectoryExists(TabDirectory))
                FileSystemService.CreateDirectory(TabDirectory);

            return String.Format("{0}\\{1}.tab", TabDirectory, Guid.NewGuid().ToString());
        }

        public static Stream CreateTabFile(string filePath)
        {
            return FileSystemService.OpenFile(filePath, FileMode.CreateNew);
        }


    }
}
