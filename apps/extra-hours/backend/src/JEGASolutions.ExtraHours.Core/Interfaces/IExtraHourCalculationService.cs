using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface IExtraHourCalculationService
    {
        Task<ExtraHourCalculation> DetermineExtraHourTypeAsync(DateTime date, TimeSpan startTime, TimeSpan endTime);
    }
}
