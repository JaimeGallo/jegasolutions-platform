using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Dto
{
    public class TemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public int? AreaId { get; set; }
        public string? AreaName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TemplateDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? AreaId { get; set; }
        public string? AreaName { get; set; }
        public TemplateConfiguration Configuration { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TemplateCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int? AreaId { get; set; }
        public TemplateConfiguration Configuration { get; set; } = new();
    }

    public class TemplateUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? AreaId { get; set; }
        public TemplateConfiguration Configuration { get; set; } = new();
    }
}
