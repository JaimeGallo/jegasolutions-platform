using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface IManagerRepository
    {
        Task<Manager?> GetByIdAsync(long manager_id);
        Task<List<Manager>> GetAllAsync();
        Task AddAsync(Manager manager);
        Task UpdateAsync(Manager manager);
        Task DeleteAsync(long manager_id);
    }
}
