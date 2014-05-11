using System;
using System.IO;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Services;

namespace PhoneGuitarTab.UI.Infrastructure
{
    public class TabFileStorage
    {
        [Dependency]
        private IFileSystemService FileSystemService { get; set; }

        private const string TabDirectory = "Tabs";

        public virtual string CreateTabFilePath()
        {
            if (!FileSystemService.DirectoryExists(TabDirectory))
                FileSystemService.CreateDirectory(TabDirectory);

            return String.Format("{0}\\{1}", TabDirectory, CreateTabFileName());
        }

        public virtual string CreateTabFileName()
        {
            return String.Format("{0}.tab", Guid.NewGuid());
        }

        public virtual Stream CreateTabFile(string filePath)
        {
            return FileSystemService.OpenFile(filePath, FileMode.CreateNew);
        }
    }
}