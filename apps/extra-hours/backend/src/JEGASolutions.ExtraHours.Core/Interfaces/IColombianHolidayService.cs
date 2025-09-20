namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface IColombianHolidayService
    {
        bool IsPublicHoliday(DateTime date);
        Task<bool> IsPublicHolidayAsync(DateTime date);
        Task<List<DateTime>> GetHolidaysInRangeAsync(DateTime startDate, DateTime endDate);
    }
}