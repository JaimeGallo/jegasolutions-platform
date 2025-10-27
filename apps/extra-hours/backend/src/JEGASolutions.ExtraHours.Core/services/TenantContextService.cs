namespace JEGASolutions.ExtraHours.Core.Services
{
    /// <summary>
    /// Implementation of tenant context service for managing tenant isolation
    /// Uses AsyncLocal to preserve tenant context across async operations
    /// </summary>
    public class TenantContextService : ITenantContextService
    {
        private static readonly AsyncLocal<int?> _currentTenantId = new AsyncLocal<int?>();

        public int GetCurrentTenantId()
        {
            if (!_currentTenantId.Value.HasValue)
                throw new InvalidOperationException("No tenant context has been set");

            return _currentTenantId.Value.Value;
        }

        public void SetCurrentTenantId(int tenantId)
        {
            if (tenantId <= 0)
                throw new ArgumentException("Tenant ID must be greater than 0", nameof(tenantId));

            _currentTenantId.Value = tenantId;
        }

        public bool HasTenantId()
        {
            return _currentTenantId.Value.HasValue;
        }
    }
}
