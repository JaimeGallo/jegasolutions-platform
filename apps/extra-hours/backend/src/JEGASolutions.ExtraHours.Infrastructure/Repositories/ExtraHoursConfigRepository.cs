using Microsoft.EntityFrameworkCore;
using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Interfaces;
using JEGASolutions.ExtraHours.Data;

namespace JEGASolutions.ExtraHours.Infrastructure.Repositories
{
    public class ExtraHoursConfigRepository : IExtraHoursConfigRepository
    {
        private readonly AppDbContext _context;

        public ExtraHoursConfigRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ExtraHoursConfig?> GetConfigAsync()
        {
            return await _context.extraHoursConfigs.FirstOrDefaultAsync();
        }

        public async Task<ExtraHoursConfig> UpdateConfigAsync(ExtraHoursConfig config)
        {
            _context.extraHoursConfigs.Update(config);
            await _context.SaveChangesAsync();
            return config;
        }
    }
}