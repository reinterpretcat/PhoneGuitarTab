using System;
using System.Collections.Generic;
using System.IO;

namespace PhoneGuitarTab.Core.Services
{
    public interface IFileSystemService
    {

        IEnumerable<string> GetFileNames(string searchPattern = null);
        IEnumerable<string> GetDirectoryNames(string searchPattern = null);

        Stream OpenFile(string path, FileMode mode);
        Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);
        bool FileExists(string fileName);
        void DeleteFile(string fileName);
        void MoveFile(string sourceFileName, string destFileName);
        DateTimeOffset GetLastWriteTime(string path);

        void CreateDirectory(string path);
        bool DirectoryExists(string path);
    }
}
