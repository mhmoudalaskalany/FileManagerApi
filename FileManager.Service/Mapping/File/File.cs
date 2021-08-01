using FileManager.Common.DTO.File;
using FileManager.Entities.Entities;

// ReSharper disable once CheckNamespace
namespace FileManager.Service.Mapping
{
    public partial class MappingService
    {
        public void MapFile()
        {
            CreateMap<File, FileDto>()
                .ReverseMap();

            CreateMap<File, AddFileDto>()
                .ReverseMap();

            CreateMap<File, DownLoadDto>().ReverseMap();
        }
    }
}