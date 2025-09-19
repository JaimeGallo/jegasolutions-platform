using JEGASolutions.ExtraHours.API.Model;

namespace JEGASolutions.ExtraHours.API.Service.Interface
{
    public interface IExtraHoursConfigService
    {
        Task<ExtraHoursConfig> GetConfigAsync();
        Task<ExtraHoursConfig> UpdateConfigAsync(ExtraHoursConfig config);
    }
}
