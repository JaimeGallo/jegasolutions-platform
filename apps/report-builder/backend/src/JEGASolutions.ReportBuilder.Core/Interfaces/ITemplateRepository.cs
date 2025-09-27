using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    public interface ITemplateRepository
    {
        Task<Template?> GetByIdAsync(int id, int tenantId);
        Task<List<Template>> GetByTenantAsync(int tenantId);
        Task<List<Template>> GetByAreaAsync(int areaId, int tenantId);
        Task<List<Template>> GetByTypeAsync(string type, int tenantId);
        Task<Template> CreateAsync(Template template);
        Task<Template> UpdateAsync(Template template);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<bool> ExistsAsync(int id, int tenantId);
    }
}
