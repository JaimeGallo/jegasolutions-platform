using Microsoft.EntityFrameworkCore;
using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Entities;
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
            // ✅ SOLUCIÓN: Asegurar que todos los DateTime sean UTC antes de guardar
            config.MarkAsUpdated(); // Esto ya establece UpdatedAt en UTC
            
            // Asegurar que CreatedAt sea UTC si existe
            if (config.CreatedAt.HasValue)
            {
                if (config.CreatedAt.Value.Kind == DateTimeKind.Unspecified)
                {
                    config.CreatedAt = DateTime.SpecifyKind(config.CreatedAt.Value, DateTimeKind.Utc);
                }
                else if (config.CreatedAt.Value.Kind == DateTimeKind.Local)
                {
                    config.CreatedAt = config.CreatedAt.Value.ToUniversalTime();
                }
            }
            
            // Asegurar que UpdatedAt sea UTC
            if (config.UpdatedAt.HasValue)
            {
                if (config.UpdatedAt.Value.Kind == DateTimeKind.Unspecified)
                {
                    config.UpdatedAt = DateTime.SpecifyKind(config.UpdatedAt.Value, DateTimeKind.Utc);
                }
                else if (config.UpdatedAt.Value.Kind == DateTimeKind.Local)
                {
                    config.UpdatedAt = config.UpdatedAt.Value.ToUniversalTime();
                }
            }
            
            // Asegurar que DeletedAt sea UTC si existe
            if (config.DeletedAt.HasValue)
            {
                if (config.DeletedAt.Value.Kind == DateTimeKind.Unspecified)
                {
                    config.DeletedAt = DateTime.SpecifyKind(config.DeletedAt.Value, DateTimeKind.Utc);
                }
                else if (config.DeletedAt.Value.Kind == DateTimeKind.Local)
                {
                    config.DeletedAt = config.DeletedAt.Value.ToUniversalTime();
                }
            }
            
            _context.extraHoursConfigs.Update(config);
            await _context.SaveChangesAsync();
            return config;
        }
    }
}
