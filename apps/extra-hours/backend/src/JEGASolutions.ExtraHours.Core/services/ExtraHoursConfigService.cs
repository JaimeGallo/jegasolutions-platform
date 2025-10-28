using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace JEGASolutions.ExtraHours.Core.Services
{
    public class ExtraHoursConfigService : IExtraHoursConfigService
    {
        private readonly IExtraHoursConfigRepository _configRepository;
        private readonly ILogger<ExtraHoursConfigService> _logger;

        public ExtraHoursConfigService(
            IExtraHoursConfigRepository configRepository,
            ILogger<ExtraHoursConfigService> logger)
        {
            _configRepository = configRepository;
            _logger = logger;
        }

        /// <summary>
        /// ✅ CRITICAL FIX: Gets configuration filtered by tenant ID
        /// This ensures proper multi-tenant data isolation
        /// </summary>
        public async Task<ExtraHoursConfig> GetConfigByTenantAsync(int tenantId)
        {
            _logger.LogInformation(
                "🔍 Buscando configuración de horas extra para tenant {TenantId}",
                tenantId);

            var config = await _configRepository.GetConfigByTenantAsync(tenantId);

            if (config == null)
            {
                _logger.LogWarning(
                    "⚠️ No se encontró configuración de horas extra para tenant {TenantId}",
                    tenantId);
                throw new KeyNotFoundException(
                    $"No existe configuración de horas extra para el tenant {tenantId}. " +
                    "Contacte al administrador del sistema.");
            }

            _logger.LogInformation(
                "✅ Configuración encontrada para tenant {TenantId}: DiurnalMultiplier={Diurnal}, NocturnalMultiplier={Nocturnal}",
                tenantId, config.diurnalMultiplier, config.nocturnalMultiplier);

            return config;
        }

        /// <summary>
        /// ⚠️ DEPRECATED: Legacy method without tenant filtering
        /// </summary>
        [Obsolete("Use GetConfigByTenantAsync instead to ensure proper multi-tenant isolation")]
        public async Task<ExtraHoursConfig> GetConfigAsync()
        {
            _logger.LogWarning("⚠️ DEPRECATED: GetConfigAsync called without tenant filtering");

            var config = await _configRepository.GetConfigAsync();
            if (config == null)
            {
                throw new KeyNotFoundException("Configuración no encontrada");
            }

            return config;
        }

        public async Task<ExtraHoursConfig> UpdateConfigAsync(ExtraHoursConfig config)
        {
            if (config.TenantId == null || config.TenantId == 0)
            {
                throw new InvalidOperationException(
                    "No se puede actualizar configuración sin TenantId válido");
            }

            _logger.LogInformation(
                "📝 Actualizando configuración para tenant {TenantId}",
                config.TenantId);

            return await _configRepository.UpdateConfigAsync(config);
        }
    }
}
