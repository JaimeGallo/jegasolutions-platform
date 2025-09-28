namespace JEGASolutions.ExtraHours.Core.Services
{
    /// <summary>
    /// Implementation of tenant context service for managing tenant isolation
    /// </summary>
    public class TenantContextService : ITenantContextService
    {
        private int? _currentTenantId;

        public int GetCurrentTenantId()
        {
            if (!_currentTenantId.HasValue)
                throw new InvalidOperationException("No tenant context has been set");

            return _currentTenantId.Value;
        }

        public void SetCurrentTenantId(int tenantId)
        {
            if (tenantId <= 0)
                throw new ArgumentException("Tenant ID must be greater than 0", nameof(tenantId));

            _currentTenantId = tenantId;
        }

        public bool HasTenantId()
        {
            return _currentTenantId.HasValue;
        }
    }
}
