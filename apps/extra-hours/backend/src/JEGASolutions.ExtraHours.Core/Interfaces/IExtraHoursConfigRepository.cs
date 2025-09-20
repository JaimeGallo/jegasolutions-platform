using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface IExtraHoursConfigRepository
    {
        Task<ExtraHoursConfig?> GetConfigAsync();
        Task<ExtraHoursConfig> UpdateConfigAsync(ExtraHoursConfig config);
    }
}
