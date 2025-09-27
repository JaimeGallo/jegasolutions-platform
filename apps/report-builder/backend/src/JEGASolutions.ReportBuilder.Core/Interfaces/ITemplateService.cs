using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Dto;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    public interface ITemplateService
    {
        Task<Template> GetTemplateByIdAsync(int templateId, int tenantId);
        Task<List<Template>> GetTemplatesByTenantAsync(int tenantId);
        Task<List<Template>> GetTemplatesByAreaAsync(int areaId, int tenantId);
        Task<List<Template>> GetTemplatesByTypeAsync(string type, int tenantId);
        Task<Template> CreateTemplateAsync(TemplateCreateDto createDto, int tenantId);
        Task<Template> UpdateTemplateAsync(TemplateUpdateDto updateDto, int tenantId);
        Task<bool> DeleteTemplateAsync(int templateId, int tenantId);
        Task<bool> TemplateExistsAsync(int templateId, int tenantId);
    }
}
