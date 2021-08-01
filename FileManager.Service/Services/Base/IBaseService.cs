using System.Collections.Generic;
using System.Threading.Tasks;
using FileManager.Common.Core;
using FileManager.Entities.Entities.Base;

namespace FileManager.Service.Services.Base
{
    public interface IBaseService<T, TDto, TGetDto, TKey , TKeyDto>
        where T : BaseEntity<TKey>
        where TDto : IEntityDto<TKeyDto>
        where TGetDto : IEntityDto<TKeyDto>
    {
        Task<IResult> GetAllAsync(bool disableTracking = false);
        Task<IResult> AddAsync(TDto model);
        Task<IResult> AddListAsync(List<TDto> model);
        Task<IResult> UpdateAsync(TDto model);
        Task<IResult> DeleteAsync(object id);
        Task<IResult> DeleteSoftAsync(object id);
        Task<IResult> GetByIdAsync(object id);
    }
}