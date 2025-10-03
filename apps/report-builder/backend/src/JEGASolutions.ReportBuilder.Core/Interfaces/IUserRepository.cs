using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<List<User>> GetAllAsync(int tenantId);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id, int tenantId);
    }
}

