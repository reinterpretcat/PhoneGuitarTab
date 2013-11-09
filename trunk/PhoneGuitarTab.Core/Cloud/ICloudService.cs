
using System.Threading.Tasks;

namespace PhoneGuitarTab.Core.Cloud
{
    public interface ICloudService
    {
        Task<OperationStatus> UploadFile(string fileName);
        Task<OperationStatus> DownloadFile(string fileName);
        Task<OperationStatus> SynchronizeFile(string fileName);
    }

    public enum OperationStatus
    {
        Completed, Failed
    }
}
