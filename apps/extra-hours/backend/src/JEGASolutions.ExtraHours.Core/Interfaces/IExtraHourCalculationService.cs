using JEGASolutions.ExtraHours.API.Model;

namespace JEGASolutions.ExtraHours.API.Service.Interface
{
    public interface IExtraHourCalculationService
    {
        Task<ExtraHourCalculation> DetermineExtraHourTypeAsync(DateTime date, TimeSpan startTime, TimeSpan endTime);
    }
}
