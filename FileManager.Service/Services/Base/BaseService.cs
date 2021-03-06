using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FileManager.Common.Abstraction.UnitOfWork;
using FileManager.Common.Core;
using FileManager.Common.DTO.Base;
using FileManager.Entities.Entities.Base;
using Microsoft.AspNetCore.Http;

namespace FileManager.Service.Services.Base
{
    public class BaseService<T, TDto, TGetDto, TKey, TKeyDto>
        : IBaseService<T, TDto, TGetDto, TKey, TKeyDto>
        where T : BaseEntity<TKey>
        where TDto : IEntityDto<TKeyDto>
        where TGetDto : IEntityDto<TKeyDto>
    {
        protected readonly IUnitOfWork<T, TKey> UnitOfWork;
        protected readonly IMapper Mapper;
        protected readonly IResponseResult ResponseResult;
        protected IResult Result;
        protected IHttpContextAccessor HttpContextAccessor;
        protected TokenClaimDto ClaimData { get; set; }

        protected internal BaseService(IServiceBaseParameter<T, TKey> businessBaseParameter)
        {
            HttpContextAccessor = businessBaseParameter.HttpContextAccessor;
            UnitOfWork = businessBaseParameter.UnitOfWork;
            ResponseResult = businessBaseParameter.ResponseResult;
            Mapper = businessBaseParameter.Mapper;
            var claims = HttpContextAccessor.HttpContext.User;
            ClaimData = new TokenClaimDto()
            {
                UserId = claims?.FindFirst(t => t.Type == "UserId")?.Value,
                Email = claims?.FindFirst(t => t.Type == "Email")?.Value
            };
        }

        public virtual async Task<IResult> GetAllAsync(bool disableTracking = false)
        {
            try
            {

                var query = await UnitOfWork.Repository.GetAllAsync(disableTracking: disableTracking);
                var data = Mapper.Map<IEnumerable<T>, IEnumerable<TGetDto>>(query);
                return ResponseResult.PostResult(data, status: HttpStatusCode.OK,
                    message: HttpStatusCode.OK.ToString());
            }
            catch (Exception e)
            {
                Result.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                Result = new ResponseResult(null, status: HttpStatusCode.InternalServerError, exception: e,
                    message: Result.Message);
                return Result;
            }
        }

        public virtual async Task<IResult> AddAsync(TDto model)
        {
            try
            {
                T entity = Mapper.Map<TDto, T>(model);
                UnitOfWork.Repository.Add(entity);
                int affectedRows = await UnitOfWork.SaveChanges();
                if (affectedRows > 0)
                {
                    Result = new ResponseResult(result: null, status: HttpStatusCode.Created,
                        message: "Data Inserted Successfully");
                }

                Result.Data = model;
                return Result;
            }
            catch (Exception e)
            {
                Result.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                Result = new ResponseResult(null, HttpStatusCode.InternalServerError, e, Result.Message);
                return Result;
            }
        }
        public virtual async Task<IResult> AddListAsync(List<TDto> model)
        {
            try
            {
                List<T> entities = Mapper.Map<List<TDto>, List<T>>(model);
                UnitOfWork.Repository.AddRange(entities);
                int affectedRows = await UnitOfWork.SaveChanges();
                if (affectedRows > 0)
                {
                    Result = new ResponseResult(result: null, status: HttpStatusCode.Created,
                        message: "Data Inserted Successfully");
                }
                Result.Data = model;
                return Result;
            }
            catch (Exception e)
            {
                Result.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                Result = new ResponseResult(null, HttpStatusCode.InternalServerError, e, Result.Message);
                return Result;
            }
        }

        public virtual async Task<IResult> UpdateAsync(TDto model)
        {
            try
            {
                T entityToUpdate = await UnitOfWork.Repository.GetAsync(model.Id);
                var newEntity = Mapper.Map(model, entityToUpdate);
                UnitOfWork.Repository.Update(entityToUpdate, newEntity);
                int affectedRows = await UnitOfWork.SaveChanges();
                if (affectedRows > 0)
                {
                    Result = ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted,
                        message: "Data Updated Successfully");
                }

                return Result;
            }
            catch (Exception e)
            {
                Result.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                Result = new ResponseResult(null, HttpStatusCode.InternalServerError, e, Result.Message);
                return Result;
            }
        }

        public virtual async Task<IResult> DeleteAsync(object id)
        {
            try
            {
                var entityToDelete = await UnitOfWork.Repository.GetAsync(id);
                UnitOfWork.Repository.Remove(entityToDelete);
                int affectedRows = await UnitOfWork.SaveChanges();
                if (affectedRows > 0)
                {
                    Result = ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted,
                        message: "Data Deleted Successfully");
                }

                return Result;
            }
            catch (Exception e)
            {
                Result.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                Result = new ResponseResult(null, HttpStatusCode.InternalServerError, e, Result.Message);
                return Result;
            }
        }
        public virtual async Task<IResult> DeleteSoftAsync(object id)
        {
            try
            {
                var entityToDelete = await UnitOfWork.Repository.GetAsync(id);
                UnitOfWork.Repository.RemoveLogical(entityToDelete);
                int affectedRows = await UnitOfWork.SaveChanges();
                if (affectedRows > 0)
                {
                    Result = ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted,
                        message: "Data Deleted Successfully");
                }

                return Result;
            }
            catch (Exception e)
            {
                Result.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                Result = new ResponseResult(null, HttpStatusCode.InternalServerError, e, Result.Message);
                return Result;
            }
        }

        public virtual async Task<IResult> GetByIdAsync(object id)
        {
            try
            {
                T query = await UnitOfWork.Repository.GetAsync(id);
                var data = Mapper.Map<T, TGetDto>(query);
                return ResponseResult.PostResult(result: data, status: HttpStatusCode.OK,
                    message: "Data Retrieved Successfully");
            }
            catch (Exception e)
            {
                Result.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
                Result = new ResponseResult(null, HttpStatusCode.InternalServerError, e, Result.Message);
                return Result;
            }
        }
    }
}
