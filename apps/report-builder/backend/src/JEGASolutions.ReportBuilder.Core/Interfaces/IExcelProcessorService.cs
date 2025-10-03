using JEGASolutions.ReportBuilder.Core.Dto;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    /// <summary>
    /// Servicio para procesamiento de archivos Excel multi-tenant
    /// </summary>
    public interface IExcelProcessorService
    {
        /// <summary>
        /// Sube y procesa archivo Excel
        /// </summary>
        Task<ExcelUploadDetailDto> UploadAndProcessExcelAsync(
            ExcelUploadCreateDto dto, 
            int userId, 
            int tenantId);

        /// <summary>
        /// Obtiene todos los uploads del tenant
        /// </summary>
        Task<List<ExcelUploadListDto>> GetExcelUploadsAsync(
            int tenantId, 
            int? areaId = null, 
            string? period = null);

        /// <summary>
        /// Obtiene detalle de upload específico
        /// </summary>
        Task<ExcelUploadDetailDto?> GetExcelUploadByIdAsync(int uploadId, int tenantId);

        /// <summary>
        /// Elimina upload (soft delete)
        /// </summary>
        Task<bool> DeleteExcelUploadAsync(int uploadId, int tenantId);

        /// <summary>
        /// Re-procesa un archivo Excel ya subido
        /// </summary>
        Task<ExcelProcessingResultDto> ReprocessExcelAsync(int uploadId, int tenantId);

        /// <summary>
        /// Extrae datos de Excel en formato JSON estructurado
        /// </summary>
        Task<object> ExtractDataFromExcelAsync(Stream excelStream, string fileName);

        /// <summary>
        /// Valida estructura de archivo Excel
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateExcelStructureAsync(
            Stream excelStream, 
            string fileName);

        /// <summary>
        /// Solicita análisis AI de datos de Excel
        /// </summary>
        Task<ExcelAIAnalysisResultDto> RequestAIAnalysisAsync(
            ExcelAIAnalysisRequestDto dto, 
            int tenantId);
    }
}

