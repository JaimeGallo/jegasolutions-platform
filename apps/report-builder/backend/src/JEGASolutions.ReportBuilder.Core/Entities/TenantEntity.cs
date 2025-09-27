using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.ReportBuilder.Core.Entities
{
    /// <summary>
    /// Base entity for multi-tenant support
    /// </summary>
    public abstract class TenantEntity
    {
        [Required]
        public int TenantId { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? DeletedAt { get; set; }
        
        public bool IsDeleted => DeletedAt.HasValue;
        
        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void MarkAsDeleted()
        {
            DeletedAt = DateTime.UtcNow;
        }
    }
}
