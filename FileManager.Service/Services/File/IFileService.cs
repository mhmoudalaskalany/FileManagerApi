using System;
using System.Threading.Tasks;
using FileManager.Common.Core;
using FileManager.Common.DTO.File;
using FileManager.Entities.Enum;
using FileManager.Service.Services.Base;
using Microsoft.AspNetCore.Http;

namespace FileManager.Service.Services.File
{
    public interface IFileService : IBaseService<FileManager.Entities.Entities.File, AddFileDto, FileDto, Guid, Guid?>
    {
        Task<IResult> UploadToSanStorage(IFormFileCollection files, StorageType storageType, bool isPublic, string appCode);
        Task<object> Download(Guid id, string appCode);
        Task<object> DownloadWithAppCode(Guid id, string token);
        Task<object> DownloadWithWaterMark(Guid id, string token);
        Task<object> GetDirectoriesAsync(StorageType storageType);
    }
}