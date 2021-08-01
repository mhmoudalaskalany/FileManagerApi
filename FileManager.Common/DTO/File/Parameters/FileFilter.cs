using System;
using FileManager.Common.DTO.Base;

namespace FileManager.Common.DTO.File.Parameters
{
    public class FileFilter : MainFilter
    {
        public Guid? Id { get; set; }
    }
}
