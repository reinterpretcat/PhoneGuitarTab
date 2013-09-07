namespace PhoneGuitarTab.Core.IsolatedStorage
{
    using System.IO;
    using System.IO.IsolatedStorage;

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
