using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FileManager.Common.Core;
using FileManager.Common.DTO.File;
using FileManager.Common.Helpers.Crypto;
using FileManager.Common.Helpers.StorageHelper;
using FileManager.Common.Helpers.Token;
using FileManager.Entities.Enum;
using FileManager.Service.Services.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FileManager.Service.Services.File
{
    public class FileService : BaseService<Entities.Entities.File, AddFileDto, FileDto, Guid, Guid?>, IFileService
    {
        private readonly IStorageService _storage;
        private readonly IConfiguration _configuration;
        private string _path;

        public FileService(IServiceBaseParameter<Entities.Entities.File, Guid> parameters, IConfiguration configuration, IStorageService storage) : base(parameters)
        {
            _configuration = configuration;
            _storage = storage;
        }


        public async Task<IResult> UploadToSanStorage(IFormFileCollection files, StorageType storageType, bool isPublic, string appCode)
        {
            var basePath = _configuration["StoragePaths:Base"];
            _path = basePath + _configuration["StoragePaths:" + appCode];
            var fileNames = (List<FileDto>)await _storage.StoreToSharedFolder(files, _path, appCode);
            fileNames.ForEach(x => x.Id = Guid.NewGuid());
            var entities = Mapper.Map<List<Entities.Entities.File>>(fileNames);
            entities.ForEach(x =>
            {
                x.CreatedDate = DateTime.UtcNow;
                x.Url = CryptoHelper.EncryptString(DateTime.UtcNow.ToString(@"dd-MM-yyyy") + @"\" + x.Url);
                x.StorageType = storageType.ToString();
                x.IsPublic = isPublic;
            });
            UnitOfWork.Repository.AddRange(entities);
            await UnitOfWork.SaveChanges();
            return new ResponseResult(fileNames, HttpStatusCode.Created, null, "AddSuccess");
        }


        public async Task<object> Download(Guid id, string appCode)
        {
            var file = await UnitOfWork.Repository.GetAsync(id);
            if (file != null)
            {
                var basePath = _configuration["StoragePaths:Base"];
                var path = basePath + _configuration["StoragePaths:" + appCode];
                var fileUrl = CryptoHelper.DecryptString(file.Url);
                var memory = (MemoryStream)await _storage.DownLoad(fileUrl, path);
                var downloadFile = Mapper.Map<DownLoadDto>(file);
                downloadFile.MemoryStream = memory;
                if (file.IsPublic)
                {
                    return downloadFile;
                }
                throw new UnauthorizedAccessException();
            }
            return null;
        }

        public async Task<object> DownloadWithAppCode(Guid id, string token)
        {
            var file = await UnitOfWork.Repository.GetAsync(id);
            if (file == null) return null;
            if (string.IsNullOrEmpty(token) || !TokenHelper.CheckToken(token, id))
                throw new UnauthorizedAccessException();
            var claims = TokenHelper.DecodeToken(token);
            var appCodeClaim = claims.FirstOrDefault(x => x.Type == "AppCode");
            var fileUrl = CryptoHelper.DecryptString(file.Url);
            var basePath = _configuration["StoragePaths:Base"];
            var path = basePath + _configuration["StoragePaths:" + appCodeClaim?.Value];
            var memory = (MemoryStream)await _storage.DownLoad(fileUrl, path);
            var downloadFile = Mapper.Map<DownLoadDto>(file);
            downloadFile.MemoryStream = memory;
            if (file.IsPublic)
            {
                return downloadFile;
            }


            return downloadFile;
        }

        public async Task<object> DownloadWithWaterMark(Guid id, string token)
        {
            var file = await UnitOfWork.Repository.GetAsync(id);
            if (file != null)
            {
                var fileUrl = CryptoHelper.DecryptString(file.Url);
                var memory = (MemoryStream)await _storage.DownLoadWithWaterMark(fileUrl, _path);
                var downloadFile = Mapper.Map<DownLoadDto>(file);
                downloadFile.MemoryStream = memory;
                if (file.IsPublic)
                {
                    return downloadFile;
                }

                if (!string.IsNullOrEmpty(token) && TokenHelper.CheckToken(token, id))
                    return downloadFile;
                throw new UnauthorizedAccessException();
            }
            return null;
        }

        public async Task<object> GetDirectoriesAsync(StorageType storageType)
        {
            var result = await _storage.GetDirectoriesAsync();
            return result;
        }
    }
}
