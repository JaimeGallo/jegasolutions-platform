using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Interfaces;

namespace JEGASolutions.ExtraHours.Core.Services
{
    public class ExtraHourService : IExtraHourService
    {
        private readonly IExtraHourRepository _extraHourRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITenantContextService _tenantContextService;

        public ExtraHourService(IExtraHourRepository extraHourRepository, IEmployeeRepository employeeRepository, ITenantContextService tenantContextService)
        {
            _extraHourRepository = extraHourRepository;
            _employeeRepository = employeeRepository;
            _tenantContextService = tenantContextService;
        }

        public async Task<IEnumerable<ExtraHour>> FindExtraHoursByIdAsync(long id)
        {
            if (!_tenantContextService.HasTenantId())
                throw new InvalidOperationException("Tenant context is required");

            var tenantId = _tenantContextService.GetCurrentTenantId();
            return await _extraHourRepository.FindExtraHoursByIdAsync(id, tenantId);
        }

        public async Task<IEnumerable<ExtraHour>> FindByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (!_tenantContextService.HasTenantId())
                throw new InvalidOperationException("Tenant context is required");

            var tenantId = _tenantContextService.GetCurrentTenantId();
            return await _extraHourRepository.FindByDateRangeAsync(startDate, endDate, tenantId);
        }

        public async Task<IEnumerable<ExtraHour>> FindExtraHoursByIdAndDateRangeAsync(long employeeId, DateTime startDate, DateTime endDate)
        {
            if (!_tenantContextService.HasTenantId())
                throw new InvalidOperationException("Tenant context is required");

            var tenantId = _tenantContextService.GetCurrentTenantId();
            return await _extraHourRepository.FindExtraHoursByIdAndDateRangeAsync(employeeId, startDate, endDate, tenantId);
        }

        public async Task<ExtraHour?> FindByRegistryAsync(long registry)
        {
            if (!_tenantContextService.HasTenantId())
                throw new InvalidOperationException("Tenant context is required");

            var tenantId = _tenantContextService.GetCurrentTenantId();
            return await _extraHourRepository.FindByRegistryAsync(registry, tenantId);
        }

        public async Task<bool> DeleteExtraHourByRegistryAsync(long registry)
        {
            if (!_tenantContextService.HasTenantId())
                throw new InvalidOperationException("Tenant context is required");

            var tenantId = _tenantContextService.GetCurrentTenantId();
            return await _extraHourRepository.DeleteByRegistryAsync(registry, tenantId);
        }

        public async Task<ExtraHour> AddExtraHourAsync(ExtraHour extraHour)
        {
            // ✅ Use tenant_id from the object if already set (controller should set this)
            // Fallback to TenantContextService if not set
            if (extraHour.TenantId == null || extraHour.TenantId == 0)
            {
                if (!_tenantContextService.HasTenantId())
                    throw new InvalidOperationException("Tenant context is required");

                var tenantId = _tenantContextService.GetCurrentTenantId();
                extraHour.TenantId = tenantId;
            }

            return await _extraHourRepository.AddAsync(extraHour);
        }

        public async Task UpdateExtraHourAsync(ExtraHour extraHour)
        {
            // ✅ Use tenant_id from the object if already set (controller should set this)
            // Fallback to TenantContextService if not set
            if (extraHour.TenantId == null || extraHour.TenantId == 0)
            {
                if (!_tenantContextService.HasTenantId())
                    throw new InvalidOperationException("Tenant context is required");

                var tenantId = _tenantContextService.GetCurrentTenantId();
                extraHour.TenantId = tenantId;
            }

            await _extraHourRepository.UpdateAsync(extraHour);
        }

        public async Task<IEnumerable<ExtraHour>> GetAllAsync()
        {
            if (!_tenantContextService.HasTenantId())
                throw new InvalidOperationException("Tenant context is required");

            var tenantId = _tenantContextService.GetCurrentTenantId();
            return await _extraHourRepository.FindAllAsync(tenantId);
        }

        public async Task<ExtraHour?> GetExtraHourWithApproverDetailsAsync(long registry)
        {
            // La lógica de negocio para incluir los detalles del aprobador ahora está en el repositorio.
            return await _extraHourRepository.FindByRegistryWithApproverAsync(registry);
        }
    }
}
