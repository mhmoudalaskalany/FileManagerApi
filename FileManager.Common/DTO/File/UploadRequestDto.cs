using FileManager.Entities.Enum;

namespace FileManager.Common.DTO.File
{
    public class UploadRequestDto
    {
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public bool IsPublic { get; set; }
        public string AttachmentExtension { get; set; }
        public StorageType StorageType { get; set; }
    }
}
