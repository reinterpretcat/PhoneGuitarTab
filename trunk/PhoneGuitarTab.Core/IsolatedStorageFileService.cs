using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneGuitarTab.Core
{
    public class IsolatedStorageFileService: IFileSystemService
    {
        private static IsolatedStorageFile _store;

        public IsolatedStorageFileService()
        {
            _store = IsolatedStorageFile.GetUserStoreForApplication();
        }

        public Stream OpenFile(string path, FileMode mode)
        {
            return _store.OpenFile(path, mode);
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
