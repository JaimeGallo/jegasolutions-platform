using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface IExtraHourRepository
    {
        Task<IEnumerable<ExtraHour>> FindExtraHoursByIdAsync(long id);
        Task<IEnumerable<ExtraHour>> FindByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ExtraHour>> FindExtraHoursByIdAndDateRangeAsync(long employeeId, DateTime startDate, DateTime endDate);
        Task<ExtraHour?> FindByRegistryAsync(long registry);
        Task<ExtraHour?> FindByRegistryWithApproverAsync(long registry);
        Task<bool> DeleteByRegistryAsync(long registry);
        Task<bool> ExistsByRegistryAsync(long registry);
        Task<ExtraHour> AddAsync(ExtraHour extraHour);
        Task UpdateAsync(ExtraHour extraHour);
        Task<IEnumerable<ExtraHour>> FindAllAsync();


    }
}
