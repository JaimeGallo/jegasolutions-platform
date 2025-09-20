using System.Collections.Generic;
using System.Threading.Tasks;
using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface ICompensationRequestService
    {
        Task<CompensationRequest> CreateAsync(CompensationRequest request);
        Task<List<CompensationRequest>> GetAllAsync();
        Task<List<CompensationRequest>> GetAllFilteredAsync(DateTime? startDate, DateTime? endDate);
        Task<List<CompensationRequest>> GetByManagerFilteredAsync(long managerId, DateTime? startDate, DateTime? endDate);
        Task<CompensationRequest?> GetByIdAsync(int id);
        Task<List<CompensationRequest>> GetByEmployeeIdAsync(long employeeId);
        Task<List<CompensationRequest>> GetByManagerIdAsync(long managerId);
        Task<CompensationRequest?> UpdateStatusAsync(int id, string status, string? justification, long? approvedById);
        Task<CompensationRequest?> UpdateAsync(CompensationRequest request);
        Task<bool> DeleteAsync(int id);
    }
}