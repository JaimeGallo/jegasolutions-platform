using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    public interface IAreaRepository
    {
        Task<Area?> GetByIdAsync(int id, int tenantId);
        Task<List<Area>> GetAllAsync(int tenantId);
        Task<Area> CreateAsync(Area area);
        Task<Area> UpdateAsync(Area area);
        Task<bool> DeleteAsync(int id, int tenantId);
    }
}

