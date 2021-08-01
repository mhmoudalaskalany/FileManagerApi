using System;
using System.Threading.Tasks;
using FileManager.Common.Abstraction.Repository;
using FileManager.Entities.Entities.Base;

namespace FileManager.Common.Abstraction.UnitOfWork
{
    public interface IUnitOfWork<T,TKey> : IDisposable where T : BaseEntity<TKey>
    {
        IRepository<T,TKey> Repository { get; }
        Task<int> SaveChanges();
        void StartTransaction();
        void CommitTransaction();
        void Rollback();
    }
}
