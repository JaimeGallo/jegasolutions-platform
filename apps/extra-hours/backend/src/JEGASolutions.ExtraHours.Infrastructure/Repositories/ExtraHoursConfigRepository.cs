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
            // Buscar la configuración existente
            var existing = await _context.extraHoursConfigs.FirstOrDefaultAsync(c => c.id == config.id);

            if (existing == null)
            {
                throw new KeyNotFoundException($"Configuración con ID {config.id} no encontrada");
            }

            // Actualizar TODOS los campos explícitamente
            existing.weeklyExtraHoursLimit = config.weeklyExtraHoursLimit;
            existing.diurnalMultiplier = config.diurnalMultiplier;
            existing.nocturnalMultiplier = config.nocturnalMultiplier;
            existing.diurnalHolidayMultiplier = config.diurnalHolidayMultiplier;
            existing.nocturnalHolidayMultiplier = config.nocturnalHolidayMultiplier;
            existing.diurnalStart = config.diurnalStart;
            existing.diurnalEnd = config.diurnalEnd;

            // Actualizar campos de auditoría (si existen en TenantEntity)
            if (existing is TenantEntity tenantEntity)
            {
                tenantEntity.MarkAsUpdated();
            }

            // Marcar la entidad como modificada explícitamente
            _context.Entry(existing).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return existing;
        }
    }
}
