using JEGASolutions.ExtraHours.API.Model;

namespace JEGASolutions.ExtraHours.API.Repositories.Interfaces
{
    public interface IExtraHoursConfigRepository
    {
        Task<ExtraHoursConfig?> GetConfigAsync();
        Task<ExtraHoursConfig> UpdateConfigAsync(ExtraHoursConfig config);
    }
}
