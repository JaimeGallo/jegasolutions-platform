using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Interfaces;

namespace JEGASolutions.ExtraHours.Core.Services
{
    public class ExtraHourService : IExtraHourService
    {
        private readonly IExtraHourRepository _extraHourRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ExtraHourService(IExtraHourRepository extraHourRepository, IEmployeeRepository employeeRepository)
        {
            _extraHourRepository = extraHourRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<ExtraHour>> FindExtraHoursByIdAsync(long id)
        {
            return await _extraHourRepository.FindExtraHoursByIdAsync(id);
        }

        public async Task<IEnumerable<ExtraHour>> FindByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _extraHourRepository.FindByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<ExtraHour>> FindExtraHoursByIdAndDateRangeAsync(long employeeId, DateTime startDate, DateTime endDate)
        {
            return await _extraHourRepository.FindExtraHoursByIdAndDateRangeAsync(employeeId, startDate, endDate);
        }

        public async Task<ExtraHour?> FindByRegistryAsync(long registry)
        {
            return await _extraHourRepository.FindByRegistryAsync(registry);
        }

        public async Task<bool> DeleteExtraHourByRegistryAsync(long registry)
        {
            return await _extraHourRepository.DeleteByRegistryAsync(registry);
        }

        public async Task<ExtraHour> AddExtraHourAsync(ExtraHour extraHour)
        {
            return await _extraHourRepository.AddAsync(extraHour);
        }

        public async Task UpdateExtraHourAsync(ExtraHour extraHour)
        {
            await _extraHourRepository.UpdateAsync(extraHour);
        }

        public async Task<IEnumerable<ExtraHour>> GetAllAsync()
        {
            return await _extraHourRepository.FindAllAsync();
        }

        public async Task<ExtraHour?> GetExtraHourWithApproverDetailsAsync(long registry)
        {
            // La lógica de negocio para incluir los detalles del aprobador ahora está en el repositorio.
            return await _extraHourRepository.FindByRegistryWithApproverAsync(registry);
        }
    }
}