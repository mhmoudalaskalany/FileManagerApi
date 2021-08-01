namespace FileManager.Common.DTO.File
{
    public class UploadResponseDto
    {
        public string AttachmentUrl { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentExtension { get; set; }
        public decimal AttachmentSize { get; set; }
        public string AttachmentType { get; set; }
    }
}
