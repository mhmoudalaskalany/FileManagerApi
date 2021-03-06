using System;

namespace FileManager.Entities.Entities.Base
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; }
        public long? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;
        public long? ModifiedById { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}