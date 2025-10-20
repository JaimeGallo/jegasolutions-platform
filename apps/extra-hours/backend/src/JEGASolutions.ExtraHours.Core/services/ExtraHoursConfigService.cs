using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Interfaces;

namespace JEGASolutions.ExtraHours.Core.Services
{
    public class ExtraHoursConfigService : IExtraHoursConfigService
    {
        private readonly IExtraHoursConfigRepository _configRepository;

        public ExtraHoursConfigService(IExtraHoursConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        public async Task<ExtraHoursConfig> GetConfigAsync()
        {
            var config = await _configRepository.GetConfigAsync();
            if (config == null)
            {
                throw new KeyNotFoundException("Configuración no encontrada");
            }

            return config;
        }

        public async Task<ExtraHoursConfig> UpdateConfigAsync(ExtraHoursConfig config)
        {
            config.id = 1L; // Asegurarse de que solo existe un registro
            return await _configRepository.UpdateConfigAsync(config);
        }
    }
}
