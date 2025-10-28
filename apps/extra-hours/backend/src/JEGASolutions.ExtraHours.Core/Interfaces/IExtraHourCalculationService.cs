using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface IExtraHourCalculationService
    {
        /// <summary>
        /// ✅ CRITICAL FIX: Calculate extra hours using tenant-specific configuration
        /// </summary>
        Task<ExtraHourCalculation> DetermineExtraHourTypeAsync(DateTime date, TimeSpan startTime, TimeSpan endTime, int tenantId);

        /// <summary>
        /// ⚠️ DEPRECATED: Legacy method without tenant filtering
        /// </summary>
        [Obsolete("Use DetermineExtraHourTypeAsync with tenantId parameter to ensure proper multi-tenant isolation")]
        Task<ExtraHourCalculation> DetermineExtraHourTypeAsync(DateTime date, TimeSpan startTime, TimeSpan endTime);
    }
}
