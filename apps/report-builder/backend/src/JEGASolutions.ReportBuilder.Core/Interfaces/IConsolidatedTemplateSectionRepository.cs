using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    /// <summary>
    /// Repositorio para ConsolidatedTemplateSection con aislamiento multi-tenant
    /// </summary>
    public interface IConsolidatedTemplateSectionRepository
    {
        /// <summary>
        /// Crea nueva sección
        /// </summary>
        Task<ConsolidatedTemplateSection> CreateAsync(ConsolidatedTemplateSection section);

        /// <summary>
        /// Actualiza sección
        /// </summary>
        Task<ConsolidatedTemplateSection> UpdateAsync(ConsolidatedTemplateSection section);

        /// <summary>
        /// Elimina sección (soft delete)
        /// </summary>
        Task<bool> DeleteAsync(int id, int tenantId);

        /// <summary>
        /// Obtiene sección por ID con filtrado por tenant
        /// </summary>
        Task<ConsolidatedTemplateSection?> GetByIdAsync(int id, int tenantId);

        /// <summary>
        /// Obtiene todas las secciones de una plantilla consolidada
        /// </summary>
        Task<List<ConsolidatedTemplateSection>> GetByTemplateIdAsync(
            int templateId, 
            int tenantId);

        /// <summary>
        /// Obtiene secciones asignadas a un área específica
        /// </summary>
        Task<List<ConsolidatedTemplateSection>> GetByAreaIdAsync(
            int areaId, 
            int tenantId, 
            string? status = null);

        /// <summary>
        /// Obtiene secciones completadas por usuario específico
        /// </summary>
        Task<List<ConsolidatedTemplateSection>> GetCompletedByUserAsync(
            int userId, 
            int tenantId);

        /// <summary>
        /// Obtiene secciones vencidas
        /// </summary>
        Task<List<ConsolidatedTemplateSection>> GetOverdueSectionsAsync(int tenantId);

        /// <summary>
        /// Obtiene secciones próximas a vencer
        /// </summary>
        Task<List<ConsolidatedTemplateSection>> GetUpcomingDeadlinesAsync(
            int tenantId, 
            int daysAhead = 7);

        /// <summary>
        /// Cuenta secciones por estado
        /// </summary>
        Task<Dictionary<string, int>> GetCountByStatusAsync(int tenantId);

        /// <summary>
        /// Cuenta secciones por área
        /// </summary>
        Task<Dictionary<int, int>> GetCountByAreaAsync(int tenantId);

        /// <summary>
        /// Obtiene progreso de plantilla (completadas vs total)
        /// </summary>
        Task<(int Total, int Completed)> GetTemplateProgressAsync(
            int templateId, 
            int tenantId);

        /// <summary>
        /// Verifica si usuario tiene acceso a sección (pertenece a su área)
        /// </summary>
        Task<bool> UserHasAccessToSectionAsync(
            int sectionId, 
            int userId, 
            int tenantId);
    }
}

