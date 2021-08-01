using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileManager.Common.DTO.File;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleImpersonation;

namespace FileManager.Common.Helpers.StorageHelper
{
    public class LocalStorageService : IStorageService
    {
        private readonly ILogger<LocalStorageService> _logger;
        private readonly IConfiguration _configuration;
        public LocalStorageService(IConfiguration configuration, ILogger<LocalStorageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<bool> Delete(string fileName, string path)
        {
            var folderPath = Path.Combine(@$"{path}images\" + fileName);
            File.Delete(folderPath);
            return true;
        }
        public async Task<object> DownLoad(string url, string path)
        {
            var username = _configuration["Network:Username"];
            var password = _configuration["Network:Password"];
            var domain = _configuration["Network:Domain"];
            var location = _configuration["StoragePaths:Location"];
            _logger.LogInformation("Username At Download:  " + username);
            _logger.LogInformation("Password At Download:  " + password);
            _logger.LogInformation("Domain At Download:  " + domain);
            _logger.LogInformation("Location At Download:  " + location);
            var credentials = new UserCredentials(domain, username, password);
            var result = Impersonation.RunAsUser(credentials, LogonType.Interactive, () =>
            {
                var folderPath = Path.Combine($"{path}" + url);
                
                var memory = new MemoryStream();
                using (var stream = new FileStream(folderPath, FileMode.Open))
                {
                     stream.CopyTo(memory);
                }
                memory.Position = 0;
                return memory;
            });

            return result;


        }

        public async Task<object> DownLoadWithWaterMark(string url, string path)
        {
            var username = _configuration["Network:Username"];
            var password = _configuration["Network:Password"];
            var domain = _configuration["Network:Domain"];
            var location = _configuration["StoragePaths:Location"];
            _logger.LogInformation("Username At Download:  " + username);
            _logger.LogInformation("Password At Download:  " + password);
            _logger.LogInformation("Domain At Download:  " + domain);
            _logger.LogInformation("Location At Download:  " + location);
            var credentials = new UserCredentials(domain, username, password);
            var result = Impersonation.RunAsUser(credentials, LogonType.Interactive, () =>
            {
                var folderPath = Path.Combine($"{path}" + url);

                var memory = new MemoryStream();
                using (var stream = new FileStream(folderPath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;
                //new code
                var bytes = memory.ToArray();
                BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);

                Font times = new Font(bfTimes, 12, Font.ITALIC, BaseColor.BLACK);
                var convertedBytes = AddWatermark(bytes, times.BaseFont, "Mahmoud Alaskalany");
                Stream stream2 = new MemoryStream(convertedBytes);
                return stream2;
                // end new code
                return memory;
            });

            return result;


        }

        private static byte[] AddWatermark(byte[] bytes, BaseFont baseFont, string watermarkText)
        {
            using (var ms = new MemoryStream(10 * 1024))
            {
                using (var reader = new PdfReader(bytes))
                using (var stamper = new PdfStamper(reader, ms))
                {
                    var pages = reader.NumberOfPages;
                    for (var i = 1; i <= pages; i++)
                    {
                        var dc = stamper.GetOverContent(i);
                        AddWaterMarkText(dc, watermarkText, baseFont, 50, 45, BaseColor.BLACK, reader.GetPageSizeWithRotation(i));
                    }
                    stamper.Close();
                }
                return ms.ToArray();
            }
        }

        public static void AddWaterMarkText(PdfContentByte pdfData, string watermarkText, BaseFont font, float fontSize, float angle, BaseColor color, Rectangle realPageSize)
        {
            var gstate = new PdfGState { FillOpacity = 0.35f, StrokeOpacity = 0.3f };
            pdfData.SaveState();
            pdfData.SetGState(gstate);
            pdfData.SetColorFill(color);
            pdfData.BeginText();
            pdfData.SetFontAndSize(font, fontSize);
            var x = (realPageSize.Right + realPageSize.Left) / 2;
            var y = (realPageSize.Bottom + realPageSize.Top) / 2;
            pdfData.ShowTextAligned(Element.ALIGN_CENTER, watermarkText, x, y, angle);
            pdfData.EndText();
            pdfData.RestoreState();
        }

        private void PdfStampInExistingFile(string watermarkImagePath, string sourceFilePath)
        {
            byte[] bytes = File.ReadAllBytes(sourceFilePath);
            var img = iTextSharp.text.Image.GetInstance(watermarkImagePath);
            img.SetAbsolutePosition(200, 400);
            PdfContentByte waterMark;

            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        waterMark = stamper.GetUnderContent(i);
                        waterMark.AddImage(img);
                    }
                }
                bytes = stream.ToArray();
            }
            File.WriteAllBytes(sourceFilePath, bytes);
        }

        public async Task<IEnumerable<object>> StoreToSharedFolder(IFormFileCollection files, string path , string appCode)
        {
            try
            {
                var username = _configuration["Network:Username"];
                var password = _configuration["Network:Password"];
                var domain = _configuration["Network:Domain"];
                var location = _configuration["StoragePaths:Location"];
                _logger.LogInformation("Username:  " + username);
                _logger.LogInformation("Password:  " + password);
                _logger.LogInformation("Domain:  " + domain);
                _logger.LogInformation("Location:  " + location);
                var credentials = new UserCredentials(domain, username, password);
                var result = Impersonation.RunAsUser(credentials, LogonType.Interactive, () =>
                {
                    var uploadsFolderPath = Path.Combine($"{path}") + DateTime.Now.Date.ToString("dd-MM-yyyy");
                    if (!Directory.Exists(uploadsFolderPath))
                        Directory.CreateDirectory(uploadsFolderPath);
                    List<FileDto> filesName = new List<FileDto>();
                    foreach (var item in files)
                    {
                        var file = new FileDto
                        {
                            Name = item.FileName, FileSize = ((item.Length / 1024f) / 1024f).ToString(),
                            AppCode = appCode
                        };
                        var newFileName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                        file.Url = newFileName;
                        file.ContentType = item.ContentType;
                        file.DocumentType = Path.GetExtension(item.FileName).Replace(".", "");
                        
                        var filePath = Path.Combine(uploadsFolderPath, newFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            item.CopyTo(stream);
                        }
                        filesName.Add(file);
                    }
                    return filesName;
                });
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(JsonConvert.SerializeObject(e.Message));
                Console.WriteLine(e);
                throw;
            }


        }
        public async Task<IEnumerable<object>> Store(IFormFileCollection files, string path)
        {
            var username = _configuration["Network:Username"];
            var password = _configuration["Network:Password"];
            var domain = _configuration["Network:Domain"];
            var location = _configuration["StoragePath"];
            _logger.LogInformation("Username At Upload:  " + username);
            _logger.LogInformation("Password At Upload:  " + password);
            _logger.LogInformation("Domain At Upload:  " + domain);
            _logger.LogInformation("Location At Upload:  " + location);
            var credentials = new UserCredentials(domain, username, password);
            var result = Impersonation.RunAsUser(credentials, LogonType.Interactive, () =>
            {
                var uploadsFolderPath = Path.Combine($"{path}") + DateTime.Now.Date.ToString("dd-MM-yyyy");
                if (!Directory.Exists(uploadsFolderPath))
                    Directory.CreateDirectory(uploadsFolderPath);
                List<FileDto> filesName = new List<FileDto>();
                foreach (var item in files)
                {
                    var file = new FileDto
                    {
                        Name = item.FileName, FileSize = ((item.Length / 1024f) / 1024f).ToString()
                    };
                    var newFileName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                    file.Url = newFileName;
                    file.ContentType = item.ContentType;
                    file.DocumentType = Path.GetExtension(item.FileName).Replace(".", "");
                    var filePath = Path.Combine(uploadsFolderPath, newFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        item.CopyTo(stream);
                    }

                    filesName.Add(file);
                }

                return filesName;
            });
            return result;

        }

        public async Task<object> StoreBytes(UploadRequestDto dto, string path)
        {
            try
            {
                var username = _configuration["Network:Username"];
                var password = _configuration["Network:Password"];
                var domain = _configuration["Network:Domain"];
                var location = _configuration["StoragePath"];
                _logger.LogInformation("Username:  " + username);
                _logger.LogInformation("Password:  " + password);
                _logger.LogInformation("Domain:  " + domain);
                _logger.LogInformation("Location:  " + location);
                var credentials = new UserCredentials(domain, username, password);
                var result = Impersonation.RunAsUser(credentials, LogonType.Interactive, () =>
                {
                    var uploadsFolderPath = Path.Combine($"{path}") + DateTime.Now.Date.ToString("dd-MM-yyyy");
                    if (!Directory.Exists(uploadsFolderPath))
                        Directory.CreateDirectory(uploadsFolderPath);

                    var file = new FileDto
                    {
                        Name = dto.FileName,
                        FileSize = ((dto.FileBytes.Length / 1024f) / 1024f).ToString()
                    };
                    var newFileName = Guid.NewGuid() + Path.GetExtension(dto.FileName) + "." + dto.AttachmentExtension;
                    file.Url = newFileName;
                    file.ContentType = dto.MimeType;
                    file.DocumentType = Path.GetExtension(dto.FileName).Replace(".", "");
                    var filePath = Path.Combine(uploadsFolderPath, newFileName);
                    using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    fs.Write(dto.FileBytes, 0, dto.FileBytes.Length);
                    return file;
                });
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task<IEnumerable<object>> GetDirectoriesAsync()
        {
            try
            {
                var username = _configuration["Network:Username"];
                var password = _configuration["Network:Password"];
                var domain = _configuration["Network:Domain"];
                var location = _configuration["StoragePaths:Location"];
                _logger.LogInformation("Username:  " + username);
                _logger.LogInformation("Password:  " + password);
                _logger.LogInformation("Domain:  " + domain);
                _logger.LogInformation("Location:  " + location);
                var credentials = new UserCredentials(domain, username, password);
                var result = Impersonation.RunAsUser(credentials, LogonType.Interactive, () => Directory.GetFiles(@location));
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(JsonConvert.SerializeObject(e.Message));
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
