using JEGASolutions.ReportBuilder.Core.Dto;
using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;

namespace JEGASolutions.ReportBuilder.Infrastructure.Services
{
    public class ConsolidatedTemplateService : IConsolidatedTemplateService
    {
        private readonly IConsolidatedTemplateRepository _templateRepository;
        private readonly IConsolidatedTemplateSectionRepository _sectionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAreaRepository _areaRepository;

        public ConsolidatedTemplateService(
            IConsolidatedTemplateRepository templateRepository,
            IConsolidatedTemplateSectionRepository sectionRepository,
            IUserRepository userRepository,
            IAreaRepository areaRepository)
        {
            _templateRepository = templateRepository;
            _sectionRepository = sectionRepository;
            _userRepository = userRepository;
            _areaRepository = areaRepository;
        }

        // ==================== SUPERUSUARIO OPERATIONS ====================

        public async Task<ConsolidatedTemplateDetailDto> CreateConsolidatedTemplateAsync(
            ConsolidatedTemplateCreateDto dto, 
            int currentUserId, 
            int tenantId)
        {
            // Validar que no exista plantilla con mismo nombre y período
            var exists = await _templateRepository.ExistsAsync(dto.Name, dto.Period, tenantId);
            if (exists)
            {
                throw new InvalidOperationException($"Ya existe una plantilla con el nombre '{dto.Name}' para el período '{dto.Period}'");
            }

            // Crear plantilla consolidada
            var template = new ConsolidatedTemplate
            {
                Name = dto.Name,
                Description = dto.Description,
                Period = dto.Period,
                Deadline = dto.Deadline,
                Status = "draft",
                ConfigurationJson = System.Text.Json.JsonSerializer.Serialize(dto.Configuration),
                CreatedByUserId = currentUserId,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            template = await _templateRepository.CreateAsync(template);

            // Crear secciones
            foreach (var sectionDto in dto.Sections)
            {
                var section = new ConsolidatedTemplateSection
                {
                    ConsolidatedTemplateId = template.Id,
                    AreaId = sectionDto.AreaId,
                    SectionTitle = sectionDto.SectionTitle,
                    SectionDescription = sectionDto.SectionDescription,
                    Status = "pending",
                    Order = sectionDto.Order,
                    SectionDeadline = sectionDto.SectionDeadline,
                    SectionConfigurationJson = System.Text.Json.JsonSerializer.Serialize(sectionDto.SectionConfiguration),
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _sectionRepository.CreateAsync(section);
            }

            // Actualizar estado a "in_progress" si tiene secciones
            if (dto.Sections.Any())
            {
                template.Status = "in_progress";
                await _templateRepository.UpdateAsync(template);
            }

            return await GetConsolidatedTemplateByIdAsync(template.Id, tenantId) 
                   ?? throw new InvalidOperationException("Error al crear la plantilla consolidada");
        }

        public async Task<ConsolidatedTemplateDetailDto> UpdateConsolidatedTemplateAsync(
            ConsolidatedTemplateUpdateDto dto, 
            int currentUserId, 
            int tenantId)
        {
            var template = await _templateRepository.GetByIdAsync(dto.Id, tenantId)
                ?? throw new InvalidOperationException("Plantilla consolidada no encontrada");

            // Validar nombre único
            var exists = await _templateRepository.ExistsAsync(dto.Name, dto.Period, tenantId, dto.Id);
            if (exists)
            {
                throw new InvalidOperationException($"Ya existe otra plantilla con el nombre '{dto.Name}' para el período '{dto.Period}'");
            }

            template.Name = dto.Name;
            template.Description = dto.Description;
            template.Period = dto.Period;
            template.Deadline = dto.Deadline;
            template.Status = dto.Status;
            template.ConfigurationJson = System.Text.Json.JsonSerializer.Serialize(dto.Configuration);

            await _templateRepository.UpdateAsync(template);

            return await GetConsolidatedTemplateByIdAsync(template.Id, tenantId)
                   ?? throw new InvalidOperationException("Error al actualizar la plantilla");
        }

        public async Task<bool> DeleteConsolidatedTemplateAsync(int templateId, int tenantId)
        {
            return await _templateRepository.DeleteAsync(templateId, tenantId);
        }

        public async Task<List<ConsolidatedTemplateListDto>> GetAllConsolidatedTemplatesAsync(
            int tenantId, 
            string? status = null)
        {
            var templates = await _templateRepository.GetAllAsync(tenantId, status);

            var result = new List<ConsolidatedTemplateListDto>();
            foreach (var template in templates)
            {
                var creator = await _userRepository.GetByIdAsync(template.CreatedByUserId);
                var progress = await _sectionRepository.GetTemplateProgressAsync(template.Id, tenantId);

                result.Add(new ConsolidatedTemplateListDto
                {
                    Id = template.Id,
                    Name = template.Name,
                    Description = template.Description,
                    Status = template.Status,
                    Period = template.Period,
                    Deadline = template.Deadline,
                    TotalSections = progress.Total,
                    CompletedSections = progress.Completed,
                    ProgressPercentage = progress.Total > 0 ? (progress.Completed * 100 / progress.Total) : 0,
                    CreatedAt = template.CreatedAt,
                    CreatedByUserName = creator?.FullName ?? "Unknown"
                });
            }

            return result;
        }

        public async Task<ConsolidatedTemplateDetailDto?> GetConsolidatedTemplateByIdAsync(
            int templateId, 
            int tenantId)
        {
            var template = await _templateRepository.GetByIdAsync(templateId, tenantId);
            if (template == null) return null;

            var sections = await _sectionRepository.GetByTemplateIdAsync(templateId, tenantId);
            var creator = await _userRepository.GetByIdAsync(template.CreatedByUserId);

            var sectionDtos = new List<ConsolidatedTemplateSectionDto>();
            foreach (var section in sections)
            {
                var area = await _areaRepository.GetByIdAsync(section.AreaId, tenantId);
                var completedBy = section.CompletedByUserId.HasValue 
                    ? await _userRepository.GetByIdAsync(section.CompletedByUserId.Value)
                    : null;

                sectionDtos.Add(new ConsolidatedTemplateSectionDto
                {
                    Id = section.Id,
                    ConsolidatedTemplateId = section.ConsolidatedTemplateId,
                    AreaId = section.AreaId,
                    AreaName = area?.Name ?? "Unknown",
                    SectionTitle = section.SectionTitle,
                    SectionDescription = section.SectionDescription,
                    Status = section.Status,
                    Order = section.Order,
                    SectionDeadline = section.SectionDeadline,
                    AssignedAt = section.AssignedAt,
                    CompletedAt = section.CompletedAt,
                    CompletedByUserId = section.CompletedByUserId,
                    CompletedByUserName = completedBy?.FullName,
                    SectionConfiguration = section.SectionConfiguration,
                    SectionData = section.SectionData
                });
            }

            return new ConsolidatedTemplateDetailDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                Status = template.Status,
                Period = template.Period,
                Deadline = template.Deadline,
                Configuration = template.Configuration,
                CreatedByUserId = template.CreatedByUserId,
                CreatedByUserName = creator?.FullName ?? "Unknown",
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt ?? template.CreatedAt,
                Sections = sectionDtos
            };
        }

        public async Task<ConsolidatedTemplateSectionDto> AddSectionToTemplateAsync(
            int templateId, 
            ConsolidatedTemplateSectionCreateDto dto, 
            int tenantId)
        {
            var template = await _templateRepository.GetByIdAsync(templateId, tenantId)
                ?? throw new InvalidOperationException("Plantilla consolidada no encontrada");

            var section = new ConsolidatedTemplateSection
            {
                ConsolidatedTemplateId = templateId,
                AreaId = dto.AreaId,
                SectionTitle = dto.SectionTitle,
                SectionDescription = dto.SectionDescription,
                Status = "pending",
                Order = dto.Order,
                SectionDeadline = dto.SectionDeadline,
                SectionConfigurationJson = System.Text.Json.JsonSerializer.Serialize(dto.SectionConfiguration),
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            section = await _sectionRepository.CreateAsync(section);

            var area = await _areaRepository.GetByIdAsync(section.AreaId, tenantId);

            return new ConsolidatedTemplateSectionDto
            {
                Id = section.Id,
                ConsolidatedTemplateId = section.ConsolidatedTemplateId,
                AreaId = section.AreaId,
                AreaName = area?.Name ?? "Unknown",
                SectionTitle = section.SectionTitle,
                SectionDescription = section.SectionDescription,
                Status = section.Status,
                Order = section.Order,
                SectionDeadline = section.SectionDeadline,
                SectionConfiguration = section.SectionConfiguration
            };
        }

        public async Task<bool> UpdateSectionStatusAsync(
            ConsolidatedTemplateSectionUpdateStatusDto dto, 
            int tenantId)
        {
            var section = await _sectionRepository.GetByIdAsync(dto.SectionId, tenantId)
                ?? throw new InvalidOperationException("Sección no encontrada");

            section.Status = dto.Status;
            await _sectionRepository.UpdateAsync(section);

            return true;
        }

        public async Task<ConsolidatedTemplateStatsDto> GetConsolidatedTemplateStatsAsync(int tenantId)
        {
            var statusCounts = await _templateRepository.GetCountByStatusAsync(tenantId);
            var sectionStatusCounts = await _sectionRepository.GetCountByStatusAsync(tenantId);
            var overdueSections = await _sectionRepository.GetOverdueSectionsAsync(tenantId);

            var stats = new ConsolidatedTemplateStatsDto
            {
                TotalTemplates = statusCounts.Values.Sum(),
                DraftTemplates = statusCounts.GetValueOrDefault("draft", 0),
                InProgressTemplates = statusCounts.GetValueOrDefault("in_progress", 0),
                CompletedTemplates = statusCounts.GetValueOrDefault("completed", 0),
                OverdueSections = overdueSections.Count,
                PendingSections = sectionStatusCounts.GetValueOrDefault("pending", 0)
            };

            // TODO: Calcular progreso por área
            stats.AreaProgress = new List<AreaProgressDto>();

            return stats;
        }

        public async Task<byte[]> ConsolidateReportAsync(
            ConsolidateReportRequestDto dto, 
            int tenantId)
        {
            var template = await _templateRepository.GetByIdAsync(dto.ConsolidatedTemplateId, tenantId)
                ?? throw new InvalidOperationException("Plantilla consolidada no encontrada");

            // TODO: Implementar generación de reporte consolidado (PDF/DOCX)
            // Por ahora, retornar array vacío
            throw new NotImplementedException("Consolidación de reporte no implementada aún");
        }

        // ==================== USER OPERATIONS ====================

        public async Task<List<MyTaskDto>> GetMyTasksAsync(int userId, int tenantId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("Usuario no encontrado");

            // TODO: Obtener AreaId del usuario
            // Por ahora, retornar lista vacía
            // En implementación completa: var sections = await _sectionRepository.GetByAreaIdAsync(user.AreaId, tenantId);
            
            var tasks = new List<MyTaskDto>();
            return tasks;
        }

        public async Task<ConsolidatedTemplateSectionDto?> GetMyTaskDetailAsync(
            int sectionId, 
            int userId, 
            int tenantId)
        {
            // Verificar acceso del usuario
            var hasAccess = await _sectionRepository.UserHasAccessToSectionAsync(sectionId, userId, tenantId);
            if (!hasAccess) return null;

            var section = await _sectionRepository.GetByIdAsync(sectionId, tenantId);
            if (section == null) return null;

            var area = await _areaRepository.GetByIdAsync(section.AreaId, tenantId);
            var completedBy = section.CompletedByUserId.HasValue 
                ? await _userRepository.GetByIdAsync(section.CompletedByUserId.Value)
                : null;

            return new ConsolidatedTemplateSectionDto
            {
                Id = section.Id,
                ConsolidatedTemplateId = section.ConsolidatedTemplateId,
                AreaId = section.AreaId,
                AreaName = area?.Name ?? "Unknown",
                SectionTitle = section.SectionTitle,
                SectionDescription = section.SectionDescription,
                Status = section.Status,
                Order = section.Order,
                SectionDeadline = section.SectionDeadline,
                AssignedAt = section.AssignedAt,
                CompletedAt = section.CompletedAt,
                CompletedByUserId = section.CompletedByUserId,
                CompletedByUserName = completedBy?.FullName,
                SectionConfiguration = section.SectionConfiguration,
                SectionData = section.SectionData
            };
        }

        public async Task<ConsolidatedTemplateSectionDto> UpdateSectionContentAsync(
            ConsolidatedTemplateSectionUpdateContentDto dto, 
            int userId, 
            int tenantId)
        {
            var hasAccess = await _sectionRepository.UserHasAccessToSectionAsync(dto.SectionId, userId, tenantId);
            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("No tienes acceso a esta sección");
            }

            var section = await _sectionRepository.GetByIdAsync(dto.SectionId, tenantId)
                ?? throw new InvalidOperationException("Sección no encontrada");

            section.SectionDataJson = System.Text.Json.JsonSerializer.Serialize(dto.SectionData);

            if (dto.MarkAsCompleted)
            {
                section.Status = "completed";
                section.CompletedAt = DateTime.UtcNow;
                section.CompletedByUserId = userId;
            }
            else if (section.Status == "pending")
            {
                section.Status = "in_progress";
            }

            await _sectionRepository.UpdateAsync(section);

            var area = await _areaRepository.GetByIdAsync(section.AreaId, tenantId);

            return new ConsolidatedTemplateSectionDto
            {
                Id = section.Id,
                ConsolidatedTemplateId = section.ConsolidatedTemplateId,
                AreaId = section.AreaId,
                AreaName = area?.Name ?? "Unknown",
                SectionTitle = section.SectionTitle,
                SectionDescription = section.SectionDescription,
                Status = section.Status,
                Order = section.Order,
                SectionDeadline = section.SectionDeadline,
                CompletedAt = section.CompletedAt,
                CompletedByUserId = section.CompletedByUserId,
                SectionConfiguration = section.SectionConfiguration,
                SectionData = section.SectionData
            };
        }

        public async Task<bool> StartWorkingOnSectionAsync(int sectionId, int userId, int tenantId)
        {
            var hasAccess = await _sectionRepository.UserHasAccessToSectionAsync(sectionId, userId, tenantId);
            if (!hasAccess) return false;

            var section = await _sectionRepository.GetByIdAsync(sectionId, tenantId);
            if (section == null) return false;

            section.Status = "in_progress";
            section.AssignedAt = DateTime.UtcNow;
            await _sectionRepository.UpdateAsync(section);

            return true;
        }

        public async Task<bool> CompleteSectionAsync(int sectionId, int userId, int tenantId)
        {
            var hasAccess = await _sectionRepository.UserHasAccessToSectionAsync(sectionId, userId, tenantId);
            if (!hasAccess) return false;

            var section = await _sectionRepository.GetByIdAsync(sectionId, tenantId);
            if (section == null) return false;

            section.Status = "completed";
            section.CompletedAt = DateTime.UtcNow;
            section.CompletedByUserId = userId;
            await _sectionRepository.UpdateAsync(section);

            return true;
        }

        // ==================== NOTIFICATIONS ====================

        public async Task<List<ConsolidatedTemplateSectionDto>> GetUpcomingDeadlinesAsync(
            int tenantId, 
            int daysAhead = 7)
        {
            var sections = await _sectionRepository.GetUpcomingDeadlinesAsync(tenantId, daysAhead);
            
            var result = new List<ConsolidatedTemplateSectionDto>();
            foreach (var section in sections)
            {
                var area = await _areaRepository.GetByIdAsync(section.AreaId, tenantId);
                
                result.Add(new ConsolidatedTemplateSectionDto
                {
                    Id = section.Id,
                    ConsolidatedTemplateId = section.ConsolidatedTemplateId,
                    AreaId = section.AreaId,
                    AreaName = area?.Name ?? "Unknown",
                    SectionTitle = section.SectionTitle,
                    Status = section.Status,
                    SectionDeadline = section.SectionDeadline
                });
            }

            return result;
        }

        public async Task<List<ConsolidatedTemplateSectionDto>> GetOverdueSectionsAsync(int tenantId)
        {
            var sections = await _sectionRepository.GetOverdueSectionsAsync(tenantId);
            
            var result = new List<ConsolidatedTemplateSectionDto>();
            foreach (var section in sections)
            {
                var area = await _areaRepository.GetByIdAsync(section.AreaId, tenantId);
                
                result.Add(new ConsolidatedTemplateSectionDto
                {
                    Id = section.Id,
                    ConsolidatedTemplateId = section.ConsolidatedTemplateId,
                    AreaId = section.AreaId,
                    AreaName = area?.Name ?? "Unknown",
                    SectionTitle = section.SectionTitle,
                    Status = section.Status,
                    SectionDeadline = section.SectionDeadline
                });
            }

            return result;
        }
    }
}

