using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace PhoneGuitarTab.Core.Services
{
    public class IsolatedStorageFileService: IFileSystemService
    {
        private static IsolatedStorageFile _store;

        public IsolatedStorageFileService()
        {
            _store = IsolatedStorageFile.GetUserStoreForApplication();
        }

        public IEnumerable<string> GetFileNames(string searchPattern = null)
        {
            return searchPattern == null?
                _store.GetFileNames() :
                _store.GetFileNames(searchPattern);
        }

        public IEnumerable<string> GetDirectoryNames(string searchPattern = null)
        {
            return searchPattern == null ?
                _store.GetDirectoryNames() :
                _store.GetDirectoryNames(searchPattern);
        }

        public Stream OpenFile(string path, FileMode mode)
        {
            return _store.OpenFile(path, mode);
        }

        public Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return _store.OpenFile(path, mode, access, share);
        }

        public bool FileExists(string fileName)
        {
            return _store.FileExists(fileName);
        }

        public void DeleteFile(string fileName)
        {
            _store.DeleteFile(fileName);
        }

        public void MoveFile(string sourceFileName, string destFileName)
        {
            _store.MoveFile(sourceFileName, destFileName);
        }

        public DateTimeOffset GetLastWriteTime(string path)
        {
            return _store.GetLastWriteTime(path);
        }

        public void CreateDirectory(string path)
        {
            if (!_store.DirectoryExists(path))
                _store.CreateDirectory(path);
        }

        public bool DirectoryExists(string path)
        {
            return _store.DirectoryExists(path);
        }
    }
}
