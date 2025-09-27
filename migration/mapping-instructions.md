# INSTRUCCIONES DE MIGRACIÓN PARA CURSOR

## PROYECTO FUENTE: migration/source-projects/report-builder-local/

### MAPEO DE ARQUITECTURA:

#### Backend (.NET):

```
DESDE: migration/source-projects/report-builder-local/backend/
HACIA: apps/report-builder/backend/

ESTRUCTURA CLEAN ARCHITECTURE:
- Controllers/ → backend/Controllers/
- Services/ → backend/Application/Services/
- Models/ → backend/Domain/Entities/
- Data/ → backend/Infrastructure/Data/
- Repositories/ → backend/Infrastructure/Repositories/
- DTOs/ → backend/Application/DTOs/
- Interfaces/ → backend/Application/Interfaces/
- Extensions/ → backend/Infrastructure/Extensions/
- Configurations/ → backend/Infrastructure/Configurations/
```

#### Frontend (React):

```
DESDE: migration/source-projects/report-builder-local/frontend/
HACIA: apps/report-builder/frontend/

ESTRUCTURA MODULAR:
- components/ → frontend/src/components/
- pages/ → frontend/src/pages/
- services/ → frontend/src/services/
- hooks/ → frontend/src/hooks/
- types/ → frontend/src/types/
- utils/ → frontend/src/utils/
- contexts/ → frontend/src/contexts/
- assets/ → frontend/src/assets/
- styles/ → frontend/src/styles/
```

### ADAPTACIONES REQUERIDAS:

#### 1. Multi-Tenant (CRÍTICO):

```csharp
// Agregar a todas las entidades del Domain
public abstract class TenantEntity
{
    public int TenantId { get; set; }
    // ... otras propiedades base
}

// Ejemplo en entidades:
public class Report : TenantEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    // TenantId viene de TenantEntity
}
```

- [ ] Agregar TenantId a todas las entidades
- [ ] Filtrar consultas por tenant en repositorios
- [ ] Middleware de tenant en controllers
- [ ] Context de tenant en frontend

#### 2. Clean Architecture:

```
Domain/ (Lógica de negocio pura)
├── Entities/ (Entidades principales)
├── ValueObjects/ (Objetos de valor)
├── Enums/ (Enumeraciones)
└── Interfaces/ (Contratos del dominio)

Application/ (Lógica de aplicación)
├── Services/ (Servicios de aplicación)
├── DTOs/ (Data Transfer Objects)
├── Interfaces/ (Contratos de servicios)
├── Mappings/ (AutoMapper profiles)
└── Validators/ (Validaciones)

Infrastructure/ (Implementaciones técnicas)
├── Data/ (DbContext, configuraciones)
├── Repositories/ (Implementación de repositorios)
├── ExternalServices/ (APIs externas, IA)
├── Extensions/ (Extensions methods)
└── Configurations/ (Configuraciones de servicios)
```

- [ ] Separar en capas: Domain, Application, Infrastructure
- [ ] Implementar interfaces en Application
- [ ] Mover lógica de negocio a Domain
- [ ] Configurar DI en Program.cs

#### 3. Integración con IA:

```csharp
// Infrastructure/ExternalServices/
public interface IAIAnalysisService
{
    Task<AIInsightDto> GenerateInsightsAsync(ReportDataDto data);
    Task<string> GenerateNarrativeAsync(ReportDto report);
}

public class OpenAIService : IAIAnalysisService
{
    // Implementación con OpenAI
}
```

- [ ] Servicios de IA en Infrastructure/ExternalServices/
- [ ] Interfaces en Application/Interfaces/
- [ ] Configuración en appsettings.json
- [ ] Variables de entorno para API keys

#### 4. Sistema de Reportes:

```csharp
// Domain/Entities/
public class Report : TenantEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ReportSection> Sections { get; set; }
    public List<AIInsight> AIInsights { get; set; }
}

public class ReportSection : TenantEntity
{
    public int Id { get; set; }
    public int ReportId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string ChartData { get; set; }
    public SectionType Type { get; set; }
}
```

#### 5. Integración con Plataforma:

- [ ] Usar shared/Services/ para servicios comunes
- [ ] Adoptar shared/Types/ para tipos compartidos
- [ ] Integrar con config/MultiTenant/ para configuración
- [ ] Seguir patrones de apps/extra-hours/
- [ ] Reutilizar ui-components/ existentes

#### 6. Frontend Multi-Tenant:

```tsx
// Contexto de Tenant
interface TenantContextType {
  tenantId: number;
  tenantInfo: TenantInfo;
  isLoading: boolean;
}

// Componentes que usen el contexto
const ReportDashboard = () => {
  const { tenantId } = useTenant();
  // Filtrar datos por tenant automáticamente
};
```

#### 7. API Controllers:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Middleware de autenticación
public class ReportsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ReportDto>>> GetReports()
    {
        var tenantId = GetTenantIdFromClaims();
        // Automáticamente filtrar por tenant
    }

    private int GetTenantIdFromClaims()
    {
        var claim = User.FindFirst("tenant_id");
        return int.Parse(claim?.Value ?? "0");
    }
}
```

### CONFIGURACIONES ESPECÍFICAS:

#### appsettings.json:

```json
{
  "ReportBuilder": {
    "OpenAI": {
      "ApiKey": "${OPENAI_API_KEY}",
      "Model": "gpt-4",
      "MaxTokens": 4000
    },
    "Storage": {
      "ReportsPath": "${REPORT_STORAGE_PATH}",
      "MaxSizeMB": 50,
      "AllowedFormats": ["PDF", "Excel", "CSV"]
    },
    "Cache": {
      "DurationMinutes": 60,
      "Enabled": true
    },
    "Export": {
      "PdfEngine": "iTextSharp",
      "ExcelEngine": "EPPlus",
      "TemplatesPath": "Templates/"
    }
  }
}
```

#### Program.cs - Registro de servicios:

```csharp
// Agregar después de services.AddExtraHours()
services.AddReportBuilderModule(configuration);

public static IServiceCollection AddReportBuilderModule(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Application Services
    services.AddScoped<IReportGenerationService, ReportGenerationService>();
    services.AddScoped<IAIAnalysisService, OpenAIService>();
    services.AddScoped<IDataConsolidationService, DataConsolidationService>();
    services.AddScoped<IReportExportService, ReportExportService>();

    // Infrastructure
    services.AddScoped<IReportRepository, ReportRepository>();

    // External Services
    services.Configure<OpenAIConfiguration>(
        configuration.GetSection("ReportBuilder:OpenAI"));

    return services;
}
```

### VALIDACIÓN POST-MIGRACIÓN:

#### Backend:

- [ ] Clean Architecture implementada correctamente
- [ ] TenantId en todas las entidades
- [ ] Repositorios filtran por tenant automáticamente
- [ ] Controllers con middleware de autenticación
- [ ] Servicios de IA funcionando
- [ ] Sistema de exportación operativo
- [ ] Tests unitarios pasan

#### Frontend:

- [ ] Componentes migrados y funcionando
- [ ] Integración con backend exitosa
- [ ] Context de tenant implementado
- [ ] Visualizaciones (charts) renderizando
- [ ] Exportación desde UI funcional
- [ ] Responsive design mantenido

#### Integración:

- [ ] APIs documentadas en Swagger
- [ ] Variables de entorno configuradas
- [ ] Migraciones de BD ejecutadas
- [ ] Shared components utilizados
- [ ] Patrones consistentes con extra-hours

### ARCHIVOS CRÍTICOS A MIGRAR:

1. **Servicios de IA** - Preservar lógica de análisis
2. **Algoritmos de consolidación** - Mantener funcionalidad de datos
3. **Componentes de visualización** - Charts y dashboards
4. **Templates de exportación** - PDFs y Excel
5. **Configuraciones específicas** - OpenAI, storage, cache

### RESULTADO ESPERADO:

- ✅ Clean Architecture consistente con extra-hours
- ✅ Multi-tenancy completamente implementado
- ✅ Funcionalidades de IA preservadas y mejoradas
- ✅ Sistema de reportes robusto y escalable
- ✅ Integración perfecta con la plataforma multi-tenant
- ✅ Preparado para comercialización con Wompi
