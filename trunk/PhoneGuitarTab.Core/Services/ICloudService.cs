using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhoneGuitarTab.Core.Services
{
    // TODO refactor interface - get rid of operation status
    public interface ICloudService
    {
        Task SignIn();

        Task<OperationStatus> CreateDirectory(string relativePath);
        Task<IEnumerable<string>> GetDirectoryNames(string relativePath);
        Task<IEnumerable<string>> GetFileNames(string relativePath);
       
        Task<OperationStatus> UploadFile(string localPath, string cloudPath);
        Task<OperationStatus> DownloadFile(string localPath, string cloudPath);

        Task<bool> FileExists(string fileName);

        void Release();
    }

    public enum OperationStatus
    {
        Completed, Failed
    }
}
