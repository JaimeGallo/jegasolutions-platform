using Microsoft.EntityFrameworkCore;
using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Interfaces;
using JEGASolutions.ExtraHours.Data;

namespace JEGASolutions.ExtraHours.Infrastructure.Repositories
{
    public class ExtraHourRepository : IExtraHourRepository
    {
        private readonly AppDbContext _context;

        public ExtraHourRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExtraHour>> FindExtraHoursByIdAsync(long id)
        {
            return await _context.extraHours
                .Include(eh => eh.ApprovedByManager)
                .Where(e => e.id == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExtraHour>> FindByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.extraHours
                .Include(eh => eh.ApprovedByManager)
                .Where(e => e.date >= startDate && e.date <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExtraHour>> FindExtraHoursByIdAndDateRangeAsync(long employeeId, DateTime startDate, DateTime endDate)
        {
            return await _context.extraHours
                .Include(eh => eh.ApprovedByManager)
                .Where(e => e.id == employeeId && e.date >= startDate && e.date <= endDate)
                .OrderByDescending(e => e.date)
                .ToListAsync();
        }

        public async Task<ExtraHour?> FindByRegistryAsync(long registry)
        {
            return await _context.extraHours
                .Include(eh => eh.ApprovedByManager)
                .FirstOrDefaultAsync(e => e.registry == registry);
        }

        public async Task<ExtraHour?> FindByRegistryWithApproverAsync(long registry)
        {
            return await _context.extraHours
                .Include(eh => eh.ApprovedByManager)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(e => e.registry == registry);
        }

        public async Task<bool> DeleteByRegistryAsync(long registry)
        {
            var extraHour = await _context.extraHours.FirstOrDefaultAsync(e => e.registry == registry);
            if (extraHour == null)
                return false;

            _context.extraHours.Remove(extraHour);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByRegistryAsync(long registry)
        {
            return await _context.extraHours.AnyAsync(e => e.registry == registry);
        }

        public async Task<ExtraHour> AddAsync(ExtraHour extraHour)
        {
            await _context.extraHours.AddAsync(extraHour);
            await _context.SaveChangesAsync();
            return extraHour;
        }

        public async Task UpdateAsync(ExtraHour extraHour)
        {
            _context.extraHours.Update(extraHour);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ExtraHour>> FindAllAsync()
        {
            return await _context.extraHours
                .Include(eh => eh.ApprovedByManager)
                .ToListAsync();
        }
    }
}