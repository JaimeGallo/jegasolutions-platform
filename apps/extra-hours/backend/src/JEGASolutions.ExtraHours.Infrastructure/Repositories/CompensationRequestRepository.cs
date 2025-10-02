using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Interfaces;
using JEGASolutions.ExtraHours.Data;
using Microsoft.EntityFrameworkCore;

namespace JEGASolutions.ExtraHours.Infrastructure.Repositories
{
    public class CompensationRequestRepository : ICompensationRequestRepository
    {
        private readonly AppDbContext _context;

        public CompensationRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CompensationRequest> AddAsync(CompensationRequest request)
        {
            await _context.compensationRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<CompensationRequest> CreateAsync(CompensationRequest request)
        {
            return await AddAsync(request);
        }

        public async Task<CompensationRequest?> GetByIdAsync(int id)
        {
            return await _context.compensationRequests
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .FirstOrDefaultAsync(cr => cr.id == id);
        }

        public async Task<List<CompensationRequest>> GetAllAsync()
        {
            return await _context.compensationRequests
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .ToListAsync();
        }

        public async Task<List<CompensationRequest>> GetAllFilteredAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.compensationRequests
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(cr => cr.RequestedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(cr => cr.RequestedAt <= endDate.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<List<CompensationRequest>> GetByManagerFilteredAsync(long managerId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.compensationRequests
                .Include(cr => cr.Employee)
                    .ThenInclude(e => e!.manager)
                .Include(cr => cr.ApprovedBy)
                .Where(cr => cr.Employee!.manager_id == managerId)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(cr => cr.RequestedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(cr => cr.RequestedAt <= endDate.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<List<CompensationRequest>> GetByEmployeeIdAsync(long employeeId)
        {
            return await _context.compensationRequests
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .Where(cr => cr.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<List<CompensationRequest>> GetByManagerIdAsync(long managerId)
        {
            return await _context.compensationRequests
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .Where(cr => cr.ApprovedById == managerId)
                .ToListAsync();
        }

        public async Task<CompensationRequest> UpdateAsync(CompensationRequest request)
        {
            _context.compensationRequests.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<CompensationRequest?> UpdateStatusAsync(int id, string status, string? justification, long? approvedById)
        {
            var request = await GetByIdAsync(id);
            if (request == null) return null;

            request.Status = status;
            request.Justification = justification;
            request.ApprovedById = approvedById;
            request.DecidedAt = DateTime.UtcNow;

            await UpdateAsync(request);
            return request;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var request = await GetByIdAsync(id);
            if (request == null) return false;

            _context.compensationRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.compensationRequests.AnyAsync(cr => cr.id == id);
        }
    }
}

