using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JEGASolutions.ExtraHours.Data;              // AppDbContext
using JEGASolutions.ExtraHours.Core.Entities.Models;     // CompensationRequest, Employee
using JEGASolutions.ExtraHours.Core.Interfaces;   // ICompensationRequestService

namespace JEGASolutions.ExtraHours.Infrastructure.Services
{
    public class CompensationRequestService : ICompensationRequestService
    {
        private readonly AppDbContext _context;

        public CompensationRequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CompensationRequest?> UpdateAsync(CompensationRequest request)
        {
            _context.CompensationRequests.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<CompensationRequest> CreateAsync(CompensationRequest request)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == request.EmployeeId);
            if (!employeeExists)
            {
                throw new InvalidOperationException($"El empleado con ID {request.EmployeeId} no existe");
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                request.Status = "Pending";
            }

            _context.CompensationRequests.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<IEnumerable<CompensationRequest>> GetAllAsync()
        {
            return await _context.CompensationRequests
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .ToListAsync();
        }

        public async Task<IEnumerable<CompensationRequest>> GetAllFilteredAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.CompensationRequests
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(cr => cr.WorkDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(cr => cr.WorkDate <= endDate.Value);

            return await query.OrderByDescending(cr => cr.RequestedAt).ToListAsync();
        }

        public async Task<IEnumerable<CompensationRequest>> GetByManagerFilteredAsync(long managerId, DateTime? startDate, DateTime? endDate)
        {
            var employees = await _context.Employees
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();

            if (!employees.Any())
                return new List<CompensationRequest>();

            var employeeIds = employees.Select(e => e.Id).ToList();

            var query = _context.CompensationRequests
                .Where(cr => employeeIds.Contains(cr.EmployeeId))
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(cr => cr.WorkDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(cr => cr.WorkDate <= endDate.Value);

            return await query.OrderByDescending(cr => cr.RequestedAt).ToListAsync();
        }

        public async Task<CompensationRequest?> GetByIdAsync(int id)
        {
            return await _context.CompensationRequests
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .FirstOrDefaultAsync(cr => cr.Id == id);
        }

        public async Task<IEnumerable<CompensationRequest>> GetByEmployeeIdAsync(long employeeId)
        {
            return await _context.CompensationRequests
                .Where(cr => cr.EmployeeId == employeeId)
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .OrderByDescending(cr => cr.RequestedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CompensationRequest>> GetByManagerIdAsync(long managerId)
        {
            var employees = await _context.Employees
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();

            if (!employees.Any())
                return new List<CompensationRequest>();

            var employeeIds = employees.Select(e => e.Id).ToList();

            return await _context.CompensationRequests
                .Where(cr => employeeIds.Contains(cr.EmployeeId))
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .OrderByDescending(cr => cr.RequestedAt)
                .ToListAsync();
        }

        public async Task<CompensationRequest?> UpdateStatusAsync(int id, string status, string? justification, long? approvedById)
        {
            var request = await _context.CompensationRequests.FindAsync(id);
            if (request == null) return null;

            request.Status = status;
            request.Justification = justification;
            request.ApprovedById = approvedById;
            request.DecidedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var request = await _context.CompensationRequests.FindAsync(id);
            if (request == null) return false;

            _context.CompensationRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
