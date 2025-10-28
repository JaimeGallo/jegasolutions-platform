using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface IExtraHoursConfigRepository
    {
        /// <summary>
        /// Gets the extra hours configuration for a specific tenant
        /// </summary>
        /// <param name="tenantId">The tenant ID to filter by</param>
        /// <returns>The configuration for the specified tenant, or null if not found</returns>
        Task<ExtraHoursConfig?> GetConfigByTenantAsync(int tenantId);

        /// <summary>
        /// Legacy method - Gets the first config without tenant filtering (deprecated)
        /// </summary>
        [Obsolete("Use GetConfigByTenantAsync instead to ensure proper multi-tenant isolation")]
        Task<ExtraHoursConfig?> GetConfigAsync();

        Task<ExtraHoursConfig> UpdateConfigAsync(ExtraHoursConfig config);
    }
}
