using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    /// <summary>
    /// Repositorio para ConsolidatedTemplate con aislamiento multi-tenant
    /// </summary>
    public interface IConsolidatedTemplateRepository
    {
        /// <summary>
        /// Crea nueva plantilla consolidada
        /// </summary>
        Task<ConsolidatedTemplate> CreateAsync(ConsolidatedTemplate template);

        /// <summary>
        /// Actualiza plantilla consolidada
        /// </summary>
        Task<ConsolidatedTemplate> UpdateAsync(ConsolidatedTemplate template);

        /// <summary>
        /// Elimina plantilla (soft delete)
        /// </summary>
        Task<bool> DeleteAsync(int id, int tenantId);

        /// <summary>
        /// Obtiene plantilla por ID con filtrado por tenant
        /// </summary>
        Task<ConsolidatedTemplate?> GetByIdAsync(int id, int tenantId);

        /// <summary>
        /// Obtiene todas las plantillas del tenant
        /// </summary>
        Task<List<ConsolidatedTemplate>> GetAllAsync(int tenantId, string? status = null);

        /// <summary>
        /// Obtiene plantillas por período
        /// </summary>
        Task<List<ConsolidatedTemplate>> GetByPeriodAsync(string period, int tenantId);

        /// <summary>
        /// Obtiene plantillas creadas por usuario específico
        /// </summary>
        Task<List<ConsolidatedTemplate>> GetByCreatorAsync(int userId, int tenantId);

        /// <summary>
        /// Cuenta plantillas por estado
        /// </summary>
        Task<Dictionary<string, int>> GetCountByStatusAsync(int tenantId);

        /// <summary>
        /// Obtiene plantillas con secciones próximas a vencer
        /// </summary>
        Task<List<ConsolidatedTemplate>> GetWithUpcomingDeadlinesAsync(
            int tenantId, 
            int daysAhead = 7);

        /// <summary>
        /// Verifica si existe plantilla con el mismo nombre y período
        /// </summary>
        Task<bool> ExistsAsync(string name, string period, int tenantId, int? excludeId = null);
    }
}

