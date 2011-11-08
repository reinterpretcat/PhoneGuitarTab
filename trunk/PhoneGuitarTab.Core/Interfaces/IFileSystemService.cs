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

namespace PhoneGuitarTab.Core
{
    public interface IFileSystemService
    {
        Stream OpenFile(string path, FileMode mode);
        void CreateDirectory(string path);
        bool DirectoryExists(string path);
    }
}
