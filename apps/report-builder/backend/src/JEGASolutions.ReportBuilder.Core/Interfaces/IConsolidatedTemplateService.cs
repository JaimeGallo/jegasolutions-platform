using JEGASolutions.ReportBuilder.Core.Dto;
using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestión de plantillas consolidadas multi-tenant
    /// </summary>
    public interface IConsolidatedTemplateService
    {
        // ==================== ADMIN OPERATIONS ====================
        
        /// <summary>
        /// Crea nueva plantilla consolidada con sus secciones (Admin only)
        /// </summary>
        Task<ConsolidatedTemplateDetailDto> CreateConsolidatedTemplateAsync(
            ConsolidatedTemplateCreateDto dto, 
            int currentUserId, 
            int tenantId);

        /// <summary>
        /// Actualiza plantilla consolidada (Admin only)
        /// </summary>
        Task<ConsolidatedTemplateDetailDto> UpdateConsolidatedTemplateAsync(
            ConsolidatedTemplateUpdateDto dto, 
            int currentUserId, 
            int tenantId);

        /// <summary>
        /// Elimina plantilla consolidada (soft delete) (Admin only)
        /// </summary>
        Task<bool> DeleteConsolidatedTemplateAsync(int templateId, int tenantId);

        /// <summary>
        /// Obtiene todas las plantillas consolidadas del tenant (Admin)
        /// </summary>
        Task<List<ConsolidatedTemplateListDto>> GetAllConsolidatedTemplatesAsync(
            int tenantId, 
            string? status = null);

        /// <summary>
        /// Obtiene detalle completo de plantilla consolidada (Admin)
        /// </summary>
        Task<ConsolidatedTemplateDetailDto?> GetConsolidatedTemplateByIdAsync(
            int templateId, 
            int tenantId);

        /// <summary>
        /// Agrega sección a plantilla existente (Admin)
        /// </summary>
        Task<ConsolidatedTemplateSectionDto> AddSectionToTemplateAsync(
            int templateId, 
            ConsolidatedTemplateSectionCreateDto dto, 
            int tenantId);

        /// <summary>
        /// Actualiza estado de sección (Admin)
        /// </summary>
        Task<bool> UpdateSectionStatusAsync(
            ConsolidatedTemplateSectionUpdateStatusDto dto, 
            int tenantId);

        /// <summary>
        /// Obtiene estadísticas de progreso (Admin Dashboard)
        /// </summary>
        Task<ConsolidatedTemplateStatsDto> GetConsolidatedTemplateStatsAsync(int tenantId);

        /// <summary>
        /// Consolida el informe final (Admin)
        /// </summary>
        Task<byte[]> ConsolidateReportAsync(
            ConsolidateReportRequestDto dto, 
            int tenantId);

        // ==================== USER OPERATIONS ====================

        /// <summary>
        /// Obtiene "Mis Tareas" - secciones asignadas al área del usuario
        /// </summary>
        Task<List<MyTaskDto>> GetMyTasksAsync(int userId, int tenantId);

        /// <summary>
        /// Obtiene detalle de una tarea específica del usuario
        /// </summary>
        Task<ConsolidatedTemplateSectionDto?> GetMyTaskDetailAsync(
            int sectionId, 
            int userId, 
            int tenantId);

        /// <summary>
        /// Actualiza contenido de sección (Usuario de área)
        /// </summary>
        Task<ConsolidatedTemplateSectionDto> UpdateSectionContentAsync(
            ConsolidatedTemplateSectionUpdateContentDto dto, 
            int userId, 
            int tenantId);

        /// <summary>
        /// Marca sección como en progreso (Usuario de área)
        /// </summary>
        Task<bool> StartWorkingOnSectionAsync(int sectionId, int userId, int tenantId);

        /// <summary>
        /// Completa sección (Usuario de área)
        /// </summary>
        Task<bool> CompleteSectionAsync(int sectionId, int userId, int tenantId);

        // ==================== NOTIFICATIONS ====================

        /// <summary>
        /// Obtiene secciones próximas a vencer (para notificaciones)
        /// </summary>
        Task<List<ConsolidatedTemplateSectionDto>> GetUpcomingDeadlinesAsync(
            int tenantId, 
            int daysAhead = 7);

        /// <summary>
        /// Obtiene secciones vencidas
        /// </summary>
        Task<List<ConsolidatedTemplateSectionDto>> GetOverdueSectionsAsync(int tenantId);
    }
}

