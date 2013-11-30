using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhoneGuitarTab.Core.Services
{
    public interface ICloudService
    {
        Task<OperationStatus> CreateDirectory(string relativePath);
        Task<IEnumerable<string>> GetDirectoryNames(string relativePath);
        Task<IEnumerable<string>> GetFileNames(string relativePath);
       
        Task<OperationStatus> UploadFile(string localPath, string cloudPath);
        Task<OperationStatus> DownloadFile(string localPath, string cloudPath);

        Task<bool> FileExists(string fileName);
    }

    public enum OperationStatus
    {
        Completed, Failed
    }
}
