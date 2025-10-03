using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.ReportBuilder.Core.Entities
{
    /// <summary>
    /// Base entity for multi-tenant support
    /// </summary>
    public abstract class TenantEntity
    {
        [Required]
        public int TenantId { get; set; } = 1; // Default to tenant 1 for single-tenant MVP
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; } // Nullable
        
        public DateTime? DeletedAt { get; set; } // Nullable (soft delete)
        
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
