namespace PhoneGuitarTab.Core.IsolatedStorage
{
    using System.IO;

    public interface IFileSystemService
    {
        Stream OpenFile(string path, FileMode mode);
        void CreateDirectory(string path);
        bool DirectoryExists(string path);
    }
}
