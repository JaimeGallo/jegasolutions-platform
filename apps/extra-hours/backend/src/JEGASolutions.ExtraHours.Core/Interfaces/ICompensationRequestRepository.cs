using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface ICompensationRequestRepository
    {
        Task<CompensationRequest> AddAsync(CompensationRequest request);
        Task<CompensationRequest> CreateAsync(CompensationRequest request); // Alias de AddAsync
        Task<CompensationRequest?> GetByIdAsync(int id); // Cambiar a int
        Task<List<CompensationRequest>> GetAllAsync();
        Task<List<CompensationRequest>> GetAllFilteredAsync(DateTime? startDate, DateTime? endDate);
        Task<List<CompensationRequest>> GetByManagerFilteredAsync(long managerId, DateTime? startDate, DateTime? endDate);
        Task<List<CompensationRequest>> GetByEmployeeIdAsync(long employeeId);
        Task<List<CompensationRequest>> GetByManagerIdAsync(long managerId);
        Task<CompensationRequest> UpdateAsync(CompensationRequest request);
        Task<CompensationRequest?> UpdateStatusAsync(int id, string status, string? justification, long? approvedById);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(long id);
    }
}