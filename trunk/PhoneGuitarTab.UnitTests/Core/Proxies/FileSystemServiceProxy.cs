using PhoneGuitarTab.Core.Services;

namespace PhoneGuitarTab.UnitTests.Core.Proxies
{
    using System.Reflection;

    using PhoneGuitarTab.Core.Dependencies.Interception.Proxies;

    public class FileSystemServiceProxy : ProxyBase, IFileSystemService
    {
        public System.IO.Stream OpenFile(System.String path, System.IO.FileMode mode)
        {
            var methodInvocation = this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), path, mode);
            return this.RunBehaviors(methodInvocation).GetReturnValue<System.IO.Stream>();
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
