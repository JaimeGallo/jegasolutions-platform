using System.Collections.Generic;
using System.Threading.Tasks;
using JEGASolutions.ExtraHours.API.Model;

namespace JEGASolutions.ExtraHours.API.Service.Interface
{
    public interface ICompensationRequestService
    {
        Task<CompensationRequest> CreateAsync(CompensationRequest request);
        Task<IEnumerable<CompensationRequest>> GetAllAsync();
        Task<IEnumerable<CompensationRequest>> GetAllFilteredAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<CompensationRequest>> GetByManagerFilteredAsync(long managerId, DateTime? startDate, DateTime? endDate);
        Task<CompensationRequest?> GetByIdAsync(int id);
        Task<IEnumerable<CompensationRequest>> GetByEmployeeIdAsync(long employeeId);
        Task<IEnumerable<CompensationRequest>> GetByManagerIdAsync(long managerId);
        Task<CompensationRequest?> UpdateStatusAsync(int id, string status, string? justification, long? approvedById);
        Task<CompensationRequest?> UpdateAsync(CompensationRequest request);
        Task<bool> DeleteAsync(int id);
    }
}