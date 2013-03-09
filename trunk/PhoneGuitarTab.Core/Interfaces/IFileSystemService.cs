using System.IO;

namespace PhoneGuitarTab.Core
{
    public interface IFileSystemService
    {
        Stream OpenFile(string path, FileMode mode);
        void CreateDirectory(string path);
        bool DirectoryExists(string path);
    }
}
