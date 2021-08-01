using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FileManager.Api.Controllers.Base;
using FileManager.Common.Core;
using FileManager.Common.DTO.File;
using FileManager.Common.Helpers.Token;
using FileManager.Entities.Enum;
using FileManager.Service.Services.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FileManager.Api.Controllers
{
    /// <summary>
    /// App Controller
    /// </summary>
    public class FilesController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;
        /// <summary>
        /// Constructor
        /// </summary>
        public FilesController(IFileService fileService, IConfiguration configuration)
        {
            _fileService = fileService;
            _configuration = configuration;
        }
        /// <summary>
        /// Get By Id 
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IResult> GetAsync(long id)
        {
            var result = await _fileService.GetByIdAsync(id);
            return result;
        }

        /// <summary>
        /// Get All 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResult> GetAllAsync()
        {
            var result = await _fileService.GetAllAsync();
            return result;
        }

        /// <summary>
        /// Download File
        /// </summary>
        /// <param name="id"></param>
        /// <param name="appCode"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadAsync(Guid id, string appCode)
        {
            var downloadFile = (DownLoadDto)await _fileService.Download(id, appCode);
            return File(downloadFile.MemoryStream, downloadFile.ContentType, downloadFile.Name);
        }

        /// <summary>
        /// Download File
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadWithAppCodeAsync(Guid id, string token)
        {
            var downloadFile = (DownLoadDto)await _fileService.DownloadWithAppCode(id, token);
            return File(downloadFile.MemoryStream, downloadFile.ContentType, downloadFile.Name);
        }

        /// <summary>
        /// Download File
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadWithWaterMark(Guid id, string token)
        {
            var downloadFile = (DownLoadDto)await _fileService.DownloadWithWaterMark(id, token);
            return File(downloadFile.MemoryStream, downloadFile.ContentType, downloadFile.Name);
        }

        /// <summary>
        /// Generate Token
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public IResult GenerateTokenAsync(List<Guid> ids)
        {
            var secretKey = _configuration.GetValue<string>("SecurityToken:SecurityKey");
            var token = TokenHelper.GenerateJsonWebToken(60, secretKey, ids);
            return new ResponseResult(token, HttpStatusCode.OK, null, "Tokens Generated Successfully");
        }

        /// <summary>
        /// Generate Token With App Code
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="appCode"></param>
        /// <returns></returns>
        [HttpPost("{appCode}")]
        public IResult GenerateTokenWithClaimsAsync(List<Guid> ids , string appCode)
        {
            var secretKey = _configuration.GetValue<string>("SecurityToken:SecurityKey");
            var token = TokenHelper.GenerateJsonWebTokenWithClaims(60, secretKey, ids , appCode);
            return new ResponseResult(token, HttpStatusCode.OK, null, "Tokens Generated Successfully");
        }

        /// <summary>
        /// Upload To San Storage
        /// </summary>
        /// <param name="files"></param>
        /// <param name="storageType"></param>
        /// <param name="isPublic"></param>
        /// <param name="appCode"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResult> UploadToSanStorageAsync(IFormFileCollection files, StorageType storageType, bool isPublic , string appCode)
        {
            var result = await _fileService.UploadToSanStorage(files, storageType, isPublic , appCode);
            return result;
        }

        /// <summary>
        /// List Current Path Directories
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDirectories(StorageType storageType)
        {
            var directories = await _fileService.GetDirectoriesAsync(storageType);
            return Ok(directories);
        }

        /// <summary>
        /// Remove  by id
        /// </summary>
        /// <param name="id">PK</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IResult> DeleteAsync(long id)
        {
            return await _fileService.DeleteAsync(id);
        }

        /// <summary>
        /// Soft Remove  by id
        /// </summary>
        /// <param name="id">PK</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IResult> DeleteSoftAsync(long id)
        {
            return await _fileService.DeleteSoftAsync(id);
        }


    }
}
