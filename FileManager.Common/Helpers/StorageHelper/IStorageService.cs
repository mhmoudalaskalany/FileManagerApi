using System.Collections.Generic;
using System.Threading.Tasks;
using FileManager.Common.DTO.File;
using Microsoft.AspNetCore.Http;

namespace FileManager.Common.Helpers.StorageHelper
{
    public interface IStorageService
    {
        Task<IEnumerable<object>> StoreToSharedFolder(IFormFileCollection files, string path , string appCode);
        Task<IEnumerable<object>> Store(IFormFileCollection files,string path);
        Task<object> StoreBytes(UploadRequestDto dto, string path);
        Task<object> DownLoad(string url, string path);
        Task<object> DownLoadWithWaterMark(string url, string path);
        Task<bool> Delete(string fileName, string path);
        Task<IEnumerable<object>> GetDirectoriesAsync();

    }
}
