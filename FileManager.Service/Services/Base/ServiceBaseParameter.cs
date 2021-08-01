using AutoMapper;
using FileManager.Common.Abstraction.UnitOfWork;
using FileManager.Common.Core;
using FileManager.Entities.Entities.Base;
using Microsoft.AspNetCore.Http;

namespace FileManager.Service.Services.Base
{
    public class ServiceBaseParameter<T,TKey> : IServiceBaseParameter<T,TKey> where T : BaseEntity<TKey>
    {

        public ServiceBaseParameter(
            IMapper mapper,
            IUnitOfWork<T, TKey> unitOfWork,
            IResponseResult responseResult,
            IHttpContextAccessor httpContextAccessor
        )
        {
            Mapper = mapper;
            UnitOfWork = unitOfWork;
            ResponseResult = responseResult;
            HttpContextAccessor = httpContextAccessor;
        }

        public IMapper Mapper { get; set; }
        public IUnitOfWork<T, TKey> UnitOfWork { get; set; }
        public IResponseResult ResponseResult { get; set; }
        public IHttpContextAccessor HttpContextAccessor { get; set; }
    }
}