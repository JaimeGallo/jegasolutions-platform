using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JEGASolutions.ExtraHours.Core.Entities.Models;     // CompensationRequest, Employee
using JEGASolutions.ExtraHours.Core.Interfaces;

namespace JEGASolutions.ExtraHours.Core.Services
{
    public class CompensationRequestService : ICompensationRequestService
    {
        private readonly ICompensationRequestRepository _repository;
        private readonly IEmployeeRepository _employeeRepository;

        public CompensationRequestService(ICompensationRequestRepository repository, IEmployeeRepository employeeRepository)
        {
            _repository = repository;
            _employeeRepository = employeeRepository;
        }

        public async Task<CompensationRequest?> UpdateAsync(CompensationRequest request)
        {
            return await _repository.UpdateAsync(request);
        }

        public async Task<CompensationRequest> CreateAsync(CompensationRequest request)
        {
            var employeeExists = await _employeeRepository.EmployeeExistsAsync(request.EmployeeId);
            if (!employeeExists)
            {
                throw new InvalidOperationException($"El empleado con ID {request.EmployeeId} no existe");
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                request.Status = "Pending";
            }

            return await _repository.CreateAsync(request);
        }

        public async Task<List<CompensationRequest>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<CompensationRequest>> GetAllFilteredAsync(DateTime? startDate, DateTime? endDate)
        {
            return await _repository.GetAllFilteredAsync(startDate, endDate);
        }

        public async Task<List<CompensationRequest>> GetByManagerFilteredAsync(long managerId, DateTime? startDate, DateTime? endDate)
        {
            return await _repository.GetByManagerFilteredAsync(managerId, startDate, endDate);
        }

        public async Task<CompensationRequest?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<CompensationRequest>> GetByEmployeeIdAsync(long employeeId)
        {
            return await _repository.GetByEmployeeIdAsync(employeeId);
        }

        public async Task<List<CompensationRequest>> GetByManagerIdAsync(long managerId)
        {
            return await _repository.GetByManagerIdAsync(managerId);
        }

        public async Task<CompensationRequest?> UpdateStatusAsync(int id, string status, string? justification, long? approvedById)
        {
            return await _repository.UpdateStatusAsync(id, status, justification, approvedById);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
