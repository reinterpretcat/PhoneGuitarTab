using System;
using System.Collections.Generic;
using System.IO;
using PhoneGuitarTab.Core.Services;

namespace PhoneGuitarTab.UnitTests.Core.Proxies
{
    using System.Reflection;

    using PhoneGuitarTab.Core.Dependencies.Interception.Proxies;

    public class FileSystemServiceProxy : ProxyBase, IFileSystemService
    {
        public IEnumerable<string> GetFileNames(string searchPattern = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetDirectoryNames(string searchPattern = null)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream OpenFile(System.String path, System.IO.FileMode mode)
        {
            var methodInvocation = this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), path, mode);
            return this.RunBehaviors(methodInvocation).GetReturnValue<System.IO.Stream>();
        }

        public Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string fileName)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public void MoveFile(string sourceFileName, string destFileName)
        {
            throw new NotImplementedException();
        }

        public DateTimeOffset GetLastWriteTime(string path)
        {
            throw new NotImplementedException();
        }

        public void CreateDirectory(System.String path)
        {
            var methodInvocation = this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), path);
            this.RunBehaviors(methodInvocation);
        }

        public System.Boolean DirectoryExists(System.String path)
        {
            var methodInvocation = this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), path);
            return this.RunBehaviors(methodInvocation).GetReturnValue<System.Boolean>();
        }

    }
}
