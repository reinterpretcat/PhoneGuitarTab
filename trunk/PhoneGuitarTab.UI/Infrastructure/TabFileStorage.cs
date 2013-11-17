using PhoneGuitarTab.Core.Services;

namespace PhoneGuitarTab.UI.Infrastructure
{
    using System;
    using System.IO;

    using PhoneGuitarTab.Core.Dependencies;

    public class TabFileStorage
    {
        [Dependency]
        private IFileSystemService FileSystemService { get; set; }

        private const string TabDirectory = "Tabs";

        public string CreateTabFilePath()
        {
            if (!this.FileSystemService.DirectoryExists(TabDirectory))
                this.FileSystemService.CreateDirectory(TabDirectory);

            return String.Format("{0}\\{1}.tab", TabDirectory, Guid.NewGuid().ToString());
        }

        public Stream CreateTabFile(string filePath)
        {
            return this.FileSystemService.OpenFile(filePath, FileMode.CreateNew);
        }


    }
}
