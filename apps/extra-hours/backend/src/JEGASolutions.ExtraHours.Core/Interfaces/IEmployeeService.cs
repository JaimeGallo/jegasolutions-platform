using JEGASolutions.ExtraHours.API.Model;
using JEGASolutions.ExtraHours.API.Dto;

namespace JEGASolutions.ExtraHours.API.Service.Interface
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
