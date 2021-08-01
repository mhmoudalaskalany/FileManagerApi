using AutoMapper;

namespace FileManager.Service.Mapping
{
    public partial class MappingService : Profile
    {
        public MappingService()
        {
            MapFile();
        }
    }
}