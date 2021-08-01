using AutoMapper;
using FileManager.Common.Abstraction.UnitOfWork;
using FileManager.Common.Core;
using FileManager.Entities.Entities.Base;
using Microsoft.AspNetCore.Http;

namespace FileManager.Service.Services.Base
{
    public interface IServiceBaseParameter<T,TKey> where T : BaseEntity<TKey>
    {
        IMapper Mapper { get; set; }
        IUnitOfWork<T,TKey> UnitOfWork { get; set; }
        IResponseResult ResponseResult { get; set; }
        IHttpContextAccessor HttpContextAccessor { get; set; }
    }
}