using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Core.Dto;

namespace JEGASolutions.ReportBuilder.Core.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _templateRepository;

        public TemplateService(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        public async Task<Template> GetTemplateByIdAsync(int templateId, int tenantId)
        {
            var template = await _templateRepository.GetByIdAsync(templateId, tenantId);
            if (template == null)
                throw new InvalidOperationException("Template not found");
            return template;
        }

        public async Task<List<Template>> GetTemplatesByTenantAsync(int tenantId)
        {
            return await _templateRepository.GetByTenantAsync(tenantId);
        }

        public async Task<List<Template>> GetTemplatesByAreaAsync(int areaId, int tenantId)
        {
            return await _templateRepository.GetByAreaAsync(areaId, tenantId);
        }

        public async Task<List<Template>> GetTemplatesByTypeAsync(string type, int tenantId)
        {
            return await _templateRepository.GetByTypeAsync(type, tenantId);
        }

        public async Task<Template> CreateTemplateAsync(TemplateCreateDto createDto, int tenantId)
        {
            var template = new Template
            {
                TenantId = tenantId,
                Name = createDto.Name,
                AreaId = createDto.AreaId,
                Configuration = createDto.Configuration
            };

            return await _templateRepository.CreateAsync(template);
        }

        public async Task<Template> UpdateTemplateAsync(TemplateUpdateDto updateDto, int tenantId)
        {
            var existingTemplate = await _templateRepository.GetByIdAsync(updateDto.Id, tenantId);
            if (existingTemplate == null)
                throw new InvalidOperationException("Template not found");

            existingTemplate.Name = updateDto.Name;
            existingTemplate.AreaId = updateDto.AreaId;
            existingTemplate.Configuration = updateDto.Configuration;
            existingTemplate.MarkAsUpdated();

            return await _templateRepository.UpdateAsync(existingTemplate);
        }

        public async Task<bool> DeleteTemplateAsync(int templateId, int tenantId)
        {
            return await _templateRepository.DeleteAsync(templateId, tenantId);
        }

        public async Task<bool> TemplateExistsAsync(int templateId, int tenantId)
        {
            return await _templateRepository.ExistsAsync(templateId, tenantId);
        }
    }
}
