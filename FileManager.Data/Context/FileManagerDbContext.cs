using FileManager.Data.DataInitializer;
using FileManager.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Data.Context
{
    public class FileManagerDbContext : DbContext
    {
        private readonly IDataInitializer _dataInitializer;
        public FileManagerDbContext(DbContextOptions<FileManagerDbContext> options, IDataInitializer dataInitializer) : base(options)
        {
            _dataInitializer = dataInitializer;
        }

        public virtual DbSet<File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Configuration

            

            #endregion

            #region Seed


            #endregion
            base.OnModelCreating(modelBuilder);
        }
    }
}
