using System;
using FileManager.Common.Core;

namespace FileManager.Common.DTO.File
{
    public class AddFileDto : IEntityDto<Guid?>
    {
        public Guid? Id { get; set; }
        public string Url { private get; set; }
        public string FileSize { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public string DocumentType { get; set; }
        public string ContentType { get; set; }
    }
}
