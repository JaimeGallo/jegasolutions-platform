using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Dto;

namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetEmployeesByManagerIdAsync(long managerId);
        Task<Employee> GetByIdAsync(long id);
        Task<List<Employee>> GetAllAsync();
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(long id, UpdateEmployeeDTO dto);
        Task DeleteEmployeeAsync(long id);
        Task<bool> EmployeeExistsAsync(long id);

    }
}
