using System;
using System.Threading.Tasks;
using FileManager.Common.Abstraction.Repository;
using FileManager.Common.Abstraction.UnitOfWork;
using FileManager.Data.Repository;
using FileManager.Entities.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FileManager.Data.UnitOfWork
{
    public class UnitOfWork<T,TKey> : IUnitOfWork<T,TKey> where T : BaseEntity<TKey>
    {
        private DbContext _context;
        private IDbContextTransaction _transaction;
        public IRepository<T,TKey> Repository { get; }
        public UnitOfWork(DbContext context)
        {
            _context = context;
            Repository = new Repository<T,TKey>(_context);
        }
        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }
        public void StartTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }
        public void CommitTransaction()
        {
            _transaction.Commit();
            _transaction.Dispose();
        }
        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (_context == null)
            {
                return;
            }

            _context.Dispose();
            _context = null;
        }
    }
}