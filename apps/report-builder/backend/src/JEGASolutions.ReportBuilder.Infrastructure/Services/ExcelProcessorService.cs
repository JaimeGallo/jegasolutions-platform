using ClosedXML.Excel;
using JEGASolutions.ReportBuilder.Core.Dto;
using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace JEGASolutions.ReportBuilder.Infrastructure.Services
{
    public class ExcelProcessorService : IExcelProcessorService
    {
        private readonly IExcelUploadRepository _excelUploadRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly ILogger<ExcelProcessorService> _logger;
        private readonly string _uploadBasePath;

        public ExcelProcessorService(
            IExcelUploadRepository excelUploadRepository,
            IAreaRepository areaRepository,
            ILogger<ExcelProcessorService> logger)
        {
            _excelUploadRepository = excelUploadRepository;
            _areaRepository = areaRepository;
            _logger = logger;
            _uploadBasePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            
            // Crear carpeta de uploads si no existe
            if (!Directory.Exists(_uploadBasePath))
            {
                Directory.CreateDirectory(_uploadBasePath);
            }
        }

        public async Task<ExcelUploadDetailDto> UploadAndProcessExcelAsync(
            ExcelUploadCreateDto dto, 
            int userId, 
            int tenantId)
        {
            try
            {
                _logger.LogInformation("Iniciando upload y procesamiento de Excel para tenant {TenantId}, área {AreaId}", 
                    tenantId, dto.AreaId);

                // Validar área existe
                var area = await _areaRepository.GetByIdAsync(dto.AreaId, tenantId);
                if (area == null)
                {
                    throw new ArgumentException($"Área {dto.AreaId} no encontrada para tenant {tenantId}");
                }

                // Decodificar archivo Base64
                byte[] fileBytes;
                try
                {
                    fileBytes = Convert.FromBase64String(dto.FileBase64);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Formato de archivo Base64 inválido", ex);
                }

                // Guardar archivo físicamente
                var tenantFolder = Path.Combine(_uploadBasePath, tenantId.ToString());
                if (!Directory.Exists(tenantFolder))
                {
                    Directory.CreateDirectory(tenantFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{dto.FileName}";
                var filePath = Path.Combine(tenantFolder, uniqueFileName);
                await File.WriteAllBytesAsync(filePath, fileBytes);

                // Crear entidad ExcelUpload
                var excelUpload = new ExcelUpload
                {
                    TenantId = tenantId,
                    AreaId = dto.AreaId,
                    FileName = dto.FileName,
                    FilePath = filePath,
                    FileSize = fileBytes.Length,
                    Period = dto.Period,
                    UploadDate = DateTime.UtcNow,
                    UploadedByUserId = userId,
                    ProcessingStatus = "processing",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var uploadedEntity = await _excelUploadRepository.CreateAsync(excelUpload);

                // Procesar Excel en background task
                try
                {
                    using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    var (isValid, errors) = await ValidateExcelStructureAsync(stream, dto.FileName);
                    
                    if (!isValid)
                    {
                        uploadedEntity.ProcessingStatus = "failed";
                        uploadedEntity.ErrorMessage = string.Join("; ", errors);
                        await _excelUploadRepository.UpdateAsync(uploadedEntity);
                        
                        return MapToDetailDto(uploadedEntity, area);
                    }

                    // Extraer datos
                    stream.Position = 0; // Reset stream
                    var extractedData = await ExtractDataFromExcelAsync(stream, dto.FileName);
                    
                    uploadedEntity.ExtractedJsonData = JsonSerializer.Serialize(extractedData);
                    uploadedEntity.ProcessingStatus = "completed";
                    uploadedEntity.ProcessedAt = DateTime.UtcNow;
                    uploadedEntity.UpdatedAt = DateTime.UtcNow;
                    
                    // Generar metadata
                    var metadata = GenerateMetadata(extractedData);
                    uploadedEntity.MetadataJson = JsonSerializer.Serialize(metadata);
                    
                    await _excelUploadRepository.UpdateAsync(uploadedEntity);

                    _logger.LogInformation("Excel procesado exitosamente: {UploadId}", uploadedEntity.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando Excel: {UploadId}", uploadedEntity.Id);
                    uploadedEntity.ProcessingStatus = "failed";
                    uploadedEntity.ErrorMessage = $"Error al procesar: {ex.Message}";
                    await _excelUploadRepository.UpdateAsync(uploadedEntity);
                }

                return MapToDetailDto(uploadedEntity, area);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en UploadAndProcessExcelAsync");
                throw;
            }
        }

        public async Task<List<ExcelUploadListDto>> GetExcelUploadsAsync(
            int tenantId, 
            int? areaId = null, 
            string? period = null)
        {
            try
            {
                var uploads = await _excelUploadRepository.GetAllAsync(tenantId);

                // Filtros opcionales
                if (areaId.HasValue)
                {
                    uploads = uploads.Where(u => u.AreaId == areaId.Value).ToList();
                }
                if (!string.IsNullOrEmpty(period))
                {
                    uploads = uploads.Where(u => u.Period.Contains(period, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                return uploads.Select(u => new ExcelUploadListDto
                {
                    Id = u.Id,
                    AreaId = u.AreaId,
                    AreaName = u.Area?.Name ?? "N/A",
                    FileName = u.FileName,
                    Period = u.Period,
                    ProcessingStatus = u.ProcessingStatus,
                    UploadDate = u.UploadDate,
                    UploadedByUserName = u.UploadedByUser?.FullName ?? "N/A",
                    HasExtractedData = !string.IsNullOrEmpty(u.ExtractedJsonData)
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetExcelUploadsAsync");
                throw;
            }
        }

        public async Task<ExcelUploadDetailDto?> GetExcelUploadByIdAsync(int uploadId, int tenantId)
        {
            try
            {
                var upload = await _excelUploadRepository.GetByIdAsync(uploadId, tenantId);
                if (upload == null) return null;

                var area = await _areaRepository.GetByIdAsync(upload.AreaId, tenantId);
                return MapToDetailDto(upload, area);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetExcelUploadByIdAsync");
                throw;
            }
        }

        public async Task<bool> DeleteExcelUploadAsync(int uploadId, int tenantId)
        {
            try
            {
                var upload = await _excelUploadRepository.GetByIdAsync(uploadId, tenantId);
                if (upload == null) return false;

                // Eliminar archivo físico
                if (File.Exists(upload.FilePath))
                {
                    File.Delete(upload.FilePath);
                }

                await _excelUploadRepository.DeleteAsync(uploadId, tenantId);
                _logger.LogInformation("Excel upload eliminado: {UploadId}", uploadId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en DeleteExcelUploadAsync");
                throw;
            }
        }

        public async Task<ExcelProcessingResultDto> ReprocessExcelAsync(int uploadId, int tenantId)
        {
            try
            {
                var upload = await _excelUploadRepository.GetByIdAsync(uploadId, tenantId);
                if (upload == null)
                {
                    return new ExcelProcessingResultDto
                    {
                        Success = false,
                        ErrorMessage = "Upload not found"
                    };
                }

                upload.ProcessingStatus = "processing";
                await _excelUploadRepository.UpdateAsync(upload);

                using var stream = new FileStream(upload.FilePath, FileMode.Open, FileAccess.Read);
                var extractedData = await ExtractDataFromExcelAsync(stream, upload.FileName);
                
                upload.ExtractedJsonData = JsonSerializer.Serialize(extractedData);
                upload.ProcessingStatus = "completed";
                upload.ProcessedAt = DateTime.UtcNow;
                upload.UpdatedAt = DateTime.UtcNow;
                
                var metadata = GenerateMetadata(extractedData);
                upload.MetadataJson = JsonSerializer.Serialize(metadata);
                
                await _excelUploadRepository.UpdateAsync(upload);

                var dataDict = extractedData as Dictionary<string, object>;
                var totalRows = dataDict?.ContainsKey("totalRows") == true 
                    ? Convert.ToInt32(dataDict["totalRows"]) 
                    : 0;

                return new ExcelProcessingResultDto
                {
                    ExcelUploadId = upload.Id,
                    Success = true,
                    ProcessingStatus = "completed",
                    ExtractedData = extractedData,
                    TotalRows = totalRows,
                    ProcessedRows = totalRows
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ReprocessExcelAsync");
                return new ExcelProcessingResultDto
                {
                    ExcelUploadId = uploadId,
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<object> ExtractDataFromExcelAsync(Stream excelStream, string fileName)
        {
            try
            {
                _logger.LogInformation("Extrayendo datos de Excel: {FileName}", fileName);

                using var workbook = new XLWorkbook(excelStream);
                var worksheet = workbook.Worksheets.First();

                var data = new List<Dictionary<string, object>>();
                var headers = new List<string>();

                // Leer headers (primera fila)
                var firstRow = worksheet.FirstRowUsed();
                foreach (var cell in firstRow.CellsUsed())
                {
                    headers.Add(cell.GetValue<string>());
                }

                // Leer datos (resto de filas)
                var dataRows = worksheet.RowsUsed().Skip(1);
                foreach (var row in dataRows)
                {
                    var rowDict = new Dictionary<string, object>();
                    int colIndex = 0;
                    
                    foreach (var cell in row.Cells(1, headers.Count))
                    {
                        var header = headers[colIndex];
                        var cellValue = cell.Value;
                        
                        // Determinar tipo de dato
                        object typedValue = cellValue.Type switch
                        {
                            XLDataType.Number => cellValue.GetNumber(),
                            XLDataType.Boolean => cellValue.GetBoolean(),
                            XLDataType.DateTime => cellValue.GetDateTime(),
                            XLDataType.TimeSpan => cellValue.GetTimeSpan(),
                            _ => cellValue.GetText()
                        };
                        
                        rowDict[header] = typedValue;
                        colIndex++;
                    }
                    
                    data.Add(rowDict);
                }

                var result = new Dictionary<string, object>
                {
                    ["fileName"] = fileName,
                    ["sheetName"] = worksheet.Name,
                    ["headers"] = headers,
                    ["data"] = data,
                    ["totalRows"] = data.Count,
                    ["totalColumns"] = headers.Count,
                    ["extractedAt"] = DateTime.UtcNow
                };

                _logger.LogInformation("Datos extraídos exitosamente: {Rows} filas, {Columns} columnas", 
                    data.Count, headers.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extrayendo datos de Excel");
                throw;
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateExcelStructureAsync(
            Stream excelStream, 
            string fileName)
        {
            var errors = new List<string>();
            
            try
            {
                using var workbook = new XLWorkbook(excelStream);
                
                if (workbook.Worksheets.Count == 0)
                {
                    errors.Add("El archivo no contiene hojas de trabajo");
                    return (false, errors);
                }

                var worksheet = workbook.Worksheets.First();
                
                if (!worksheet.RowsUsed().Any())
                {
                    errors.Add("La hoja de trabajo está vacía");
                    return (false, errors);
                }

                var firstRow = worksheet.FirstRowUsed();
                if (!firstRow.CellsUsed().Any())
                {
                    errors.Add("La primera fila (headers) está vacía");
                    return (false, errors);
                }

                // Validar que hay al menos una fila de datos
                if (worksheet.RowsUsed().Count() < 2)
                {
                    errors.Add("El archivo debe contener al menos una fila de datos además de los headers");
                    return (false, errors);
                }

                await Task.CompletedTask;
                return (true, errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Error al validar estructura: {ex.Message}");
                return (false, errors);
            }
        }

        public async Task<ExcelAIAnalysisResultDto> RequestAIAnalysisAsync(
            ExcelAIAnalysisRequestDto dto, 
            int tenantId)
        {
            try
            {
                var upload = await _excelUploadRepository.GetByIdAsync(dto.ExcelUploadId, tenantId);
                if (upload == null || string.IsNullOrEmpty(upload.ExtractedJsonData))
                {
                    throw new ArgumentException("Upload not found or no extracted data available");
                }

                // TODO: Integrar con MultiAIService cuando esté implementado
                // Por ahora retornar resultado simulado
                
                _logger.LogInformation(
                    "Análisis AI solicitado para upload {UploadId} usando proveedor {Provider}", 
                    dto.ExcelUploadId, dto.AIProvider);

                return new ExcelAIAnalysisResultDto
                {
                    ExcelUploadId = dto.ExcelUploadId,
                    AIProvider = dto.AIProvider,
                    AnalysisType = dto.AnalysisType,
                    InsightText = "Análisis AI pendiente de implementación completa",
                    GeneratedAt = DateTime.UtcNow,
                    Confidence = 0.85m,
                    KeyFindings = new List<string> 
                    { 
                        "Análisis en desarrollo",
                        "Integración con MultiAIService pendiente"
                    },
                    Recommendations = new List<string> 
                    { 
                        "Implementar llamadas a AI providers" 
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en RequestAIAnalysisAsync");
                throw;
            }
        }

        // Helper methods
        private ExcelUploadDetailDto MapToDetailDto(ExcelUpload upload, Area? area)
        {
            object? parsedData = null;
            if (!string.IsNullOrEmpty(upload.ExtractedJsonData))
            {
                try
                {
                    parsedData = JsonSerializer.Deserialize<object>(upload.ExtractedJsonData);
                }
                catch
                {
                    parsedData = null;
                }
            }

            return new ExcelUploadDetailDto
            {
                Id = upload.Id,
                AreaId = upload.AreaId,
                AreaName = area?.Name ?? "N/A",
                FileName = upload.FileName,
                FilePath = upload.FilePath,
                Period = upload.Period,
                ProcessingStatus = upload.ProcessingStatus,
                UploadDate = upload.UploadDate,
                UploadedByUserId = upload.UploadedByUserId,
                UploadedByUserName = upload.UploadedByUser?.FullName ?? "N/A",
                ExtractedJsonData = upload.ExtractedJsonData,
                ParsedData = parsedData,
                ErrorMessage = upload.ErrorMessage,
                CreatedAt = upload.CreatedAt
            };
        }

        private Dictionary<string, object> GenerateMetadata(object extractedData)
        {
            var metadata = new Dictionary<string, object>
            {
                ["processedAt"] = DateTime.UtcNow,
                ["processorVersion"] = "1.0.0"
            };

            if (extractedData is Dictionary<string, object> dataDict)
            {
                if (dataDict.ContainsKey("totalRows"))
                    metadata["totalRows"] = dataDict["totalRows"];
                if (dataDict.ContainsKey("totalColumns"))
                    metadata["totalColumns"] = dataDict["totalColumns"];
                if (dataDict.ContainsKey("sheetName"))
                    metadata["sheetName"] = dataDict["sheetName"];
            }

            return metadata;
        }
    }
}

