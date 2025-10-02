using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.ExtraHours.Core.Entities
{
    /// <summary>
    /// Base entity for multi-tenant support in Extra-Hours module
    /// </summary>
    public abstract class TenantEntity
    {
        // Make TenantId optional for backwards compatibility with existing data
        public int? TenantId { get; set; } = 1; // Default to tenant 1
        
        // Make audit fields optional for backwards compatibility
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        
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
