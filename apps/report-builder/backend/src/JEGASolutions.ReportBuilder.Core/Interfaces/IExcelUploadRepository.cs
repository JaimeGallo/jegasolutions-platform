using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    /// <summary>
    /// Repositorio para ExcelUpload con aislamiento multi-tenant
    /// </summary>
    public interface IExcelUploadRepository
    {
        /// <summary>
        /// Crea nuevo registro de upload
        /// </summary>
        Task<ExcelUpload> CreateAsync(ExcelUpload upload);

        /// <summary>
        /// Actualiza registro de upload
        /// </summary>
        Task<ExcelUpload> UpdateAsync(ExcelUpload upload);

        /// <summary>
        /// Elimina upload (soft delete)
        /// </summary>
        Task<bool> DeleteAsync(int id, int tenantId);

        /// <summary>
        /// Obtiene upload por ID con filtrado por tenant
        /// </summary>
        Task<ExcelUpload?> GetByIdAsync(int id, int tenantId);

        /// <summary>
        /// Obtiene todos los uploads del tenant
        /// </summary>
        Task<List<ExcelUpload>> GetAllAsync(
            int tenantId, 
            int? areaId = null, 
            string? period = null);

        /// <summary>
        /// Obtiene uploads por área
        /// </summary>
        Task<List<ExcelUpload>> GetByAreaIdAsync(int areaId, int tenantId);

        /// <summary>
        /// Obtiene uploads por período
        /// </summary>
        Task<List<ExcelUpload>> GetByPeriodAsync(string period, int tenantId);

        /// <summary>
        /// Obtiene uploads subidos por usuario específico
        /// </summary>
        Task<List<ExcelUpload>> GetByUploaderAsync(int userId, int tenantId);

        /// <summary>
        /// Obtiene uploads por estado de procesamiento
        /// </summary>
        Task<List<ExcelUpload>> GetByProcessingStatusAsync(
            string status, 
            int tenantId);

        /// <summary>
        /// Obtiene uploads pendientes de procesar
        /// </summary>
        Task<List<ExcelUpload>> GetPendingProcessingAsync(int tenantId);

        /// <summary>
        /// Cuenta uploads por estado
        /// </summary>
        Task<Dictionary<string, int>> GetCountByStatusAsync(int tenantId);

        /// <summary>
        /// Verifica si existe upload con mismo nombre y período
        /// </summary>
        Task<bool> ExistsAsync(
            string fileName, 
            string period, 
            int areaId, 
            int tenantId);
    }
}

