namespace JEGASolutions.ExtraHours.Core.Services
{
    /// <summary>
    /// Service for managing tenant context in multi-tenant applications
    /// </summary>
    public interface ITenantContextService
    {
        /// <summary>
        /// Gets the current tenant ID from the context
        /// </summary>
        int GetCurrentTenantId();

        /// <summary>
        /// Sets the current tenant ID
        /// </summary>
        void SetCurrentTenantId(int tenantId);

        /// <summary>
        /// Checks if a tenant ID is set
        /// </summary>
        bool HasTenantId();
    }
}
