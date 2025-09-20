using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Dto;


namespace JEGASolutions.ExtraHours.Core.Interfaces

{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetEmployeesByManagerIdAsync(long managerId);
        Task<Employee?> GetByIdAsync(long id);
        Task<List<Employee>> GetAllAsync();
        Task<Employee> AddAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(long id, UpdateEmployeeDTO dto);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(long id);
        Task<bool> EmployeeExistsAsync(long id);
        Task<bool> ExistsAsync(long id);

    }
}
