using System.Collections.Generic;
using System.Threading.Tasks;
using JEGASolutions.ExtraHours.Core.Entities.Models;     // Employee
using JEGASolutions.ExtraHours.Core.Dto;
using JEGASolutions.ExtraHours.Core.Interfaces;   // IEmployeeService, IEmployeeRepository, IManagerRepository

namespace JEGASolutions.ExtraHours.Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IManagerRepository _managerRepository;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IManagerRepository managerRepository)
        {
            _employeeRepository = employeeRepository;
            _managerRepository = managerRepository;
        }

        public async Task<bool> EmployeeExistsAsync(long id)
        {
            return await _employeeRepository.EmployeeExistsAsync(id);
        }

        public async Task<List<Employee>> GetEmployeesByManagerIdAsync(long managerId)
        {
            return await _employeeRepository.GetEmployeesByManagerIdAsync(managerId);
        }

        public async Task<Employee> GetByIdAsync(long id)
        {
            return await _employeeRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Empleado no encontrado");
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            return await _employeeRepository.AddAsync(employee);
        }

        public async Task<Employee> UpdateEmployeeAsync(long id, UpdateEmployeeDTO dto)
        {
            return await _employeeRepository.UpdateEmployeeAsync(id, dto);
        }

        public async Task DeleteEmployeeAsync(long id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                throw new KeyNotFoundException("Empleado no encontrado");
            }

            await _employeeRepository.DeleteAsync(employee.id);
        }
    }
}
