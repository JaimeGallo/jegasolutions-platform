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

        public async Task<ExtraHoursConfig> GetConfigAsync()
        {
            var config = await _configRepository.GetConfigAsync();
            if (config == null)
            {
                _logger.LogWarning("⚠️ Configuración no encontrada");
                throw new KeyNotFoundException("Configuración no encontrada");
            }

            _logger.LogInformation($"✅ Config retrieved: weeklyLimit={config.weeklyExtraHoursLimit}, diurnalEnd={config.diurnalEnd}");
            return config;
        }

        public async Task<ExtraHoursConfig> UpdateConfigAsync(ExtraHoursConfig config)
        {
            try
            {
                config.id = 1L; // Asegurarse de que solo existe un registro
                
                _logger.LogInformation($"🔄 Updating config: weeklyLimit={config.weeklyExtraHoursLimit}");
                
                var result = await _configRepository.UpdateConfigAsync(config);
                
                _logger.LogInformation($"✅ Config updated successfully");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating config");
                throw;
            }
        }
    }
}
