using System.IO;

namespace FileManager.Common.DTO.File
{
    public class DownLoadDto
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public bool IsPublic { get; set; }
        public MemoryStream MemoryStream { get; set; }
    }
}
