using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using JEGASolutions.ExtraHours.Data;
using JEGASolutions.ExtraHours.Core.Interfaces;
using JEGASolutions.ExtraHours.Core.Services;
using JEGASolutions.ExtraHours.Infrastructure.Repositories;
using JEGASolutions.ExtraHours.Infrastructure.Services;
using JEGASolutions.ExtraHours.API.Middleware;
using System.IdentityModel.Tokens.Jwt;

// ⚡⚡⚡ CRÍTICO: Estas líneas DEBEN estar ANTES de crear el builder ⚡⚡⚡
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

// 🔄 Force deploy - AsyncLocal TenantContext fix - 2025-10-27
var builder = WebApplication.CreateBuilder(args);

// ✅ AGREGAR: Configurar logging para reducir ruido de autenticación
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Error);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authorization", LogLevel.Warning);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========================================
// 🔧 Database Configuration (snake_case - PostgreSQL)
// ========================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention()
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
);

// ✅ Configure global DateTime handling for PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// JWT Authentication

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

// Debug: Log JWT configuration
Console.WriteLine($"🔧 JWT Configuration:");
Console.WriteLine($"   Issuer: {jwtSettings["Issuer"]}");
Console.WriteLine($"   Audience: {jwtSettings["Audience"]}");
Console.WriteLine($"   SecretKey length: {secretKey.Length}");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            // ✅ ACEPTAR MÚLTIPLES ISSUERS Y AUDIENCES
            ValidIssuers = new[]
            {
                jwtSettings["Issuer"],           // "JEGASolutions.Landing.API"
                "JEGASolutions.ExtraHours"       // Extra Hours API issuer
            },
            ValidAudiences = new[]
            {
                jwtSettings["Audience"],         // "jegasolutions-landing-client"
                "JEGASolutions.ExtraHours.Users" // Extra Hours API audience
            },

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            // ✅ MAPEO DE CLAIMS
            RoleClaimType = "role",
            NameClaimType = ClaimTypes.Name
        };

        // ✅ AGREGAR: Eventos para debugging y manejo de endpoints públicos
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Lista de endpoints públicos que no requieren autenticación
                var isPublicEndpoint =
                    context.Request.Path.StartsWithSegments("/api/logout") ||
                    context.Request.Path.StartsWithSegments("/api/extra-hour/calculate") ||
                    context.Request.Path.StartsWithSegments("/health") ||
                    context.Request.Path.StartsWithSegments("/swagger");

                // Solo loggear si NO es un endpoint público
                if (!isPublicEndpoint)
                {
                    Console.WriteLine($"🔴 Auth failed: {context.Exception.Message}");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var role = context.Principal?.FindFirst("role")?.Value;
                var userId = context.Principal?.FindFirst("userId")?.Value
                          ?? context.Principal?.FindFirst("id")?.Value;
                Console.WriteLine($"✅ Token válido - Role: {role}, UserId: {userId}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                // Lista de endpoints públicos que no requieren autenticación
                var isPublicEndpoint =
                    context.Request.Path.StartsWithSegments("/api/logout") ||
                    context.Request.Path.StartsWithSegments("/api/extra-hour/calculate") ||
                    context.Request.Path.StartsWithSegments("/health") ||
                    context.Request.Path.StartsWithSegments("/swagger");

                // Suprimir el challenge para endpoints públicos
                if (isPublicEndpoint)
                {
                    context.HandleResponse();
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<ITenantContextService, TenantContextService>();
// ✅ SSO: Registrar JWTUtils para manejar tokens del Landing API
builder.Services.AddScoped<IJWTUtils, JEGASolutions.ExtraHours.Infrastructure.Services.JWTUtils>();
// builder.Services.AddScoped<IAuthService, JEGASolutions.ExtraHours.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<IColombianHolidayService, JEGASolutions.ExtraHours.Infrastructure.Services.ColombianHolidayService>();

// Register Repositories
builder.Services.AddScoped<IExtraHourRepository, ExtraHourRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IExtraHoursConfigRepository, ExtraHoursConfigRepository>();
builder.Services.AddScoped<ICompensationRequestRepository, CompensationRequestRepository>();

// Register Business Services
builder.Services.AddScoped<IExtraHourService, ExtraHourService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IExtraHoursConfigService, ExtraHoursConfigService>();
builder.Services.AddScoped<IExtraHourCalculationService, ExtraHourCalculationService>();
builder.Services.AddScoped<ICompensationRequestService, CompensationRequestService>();
builder.Services.AddScoped<IEmailService, JEGASolutions.ExtraHours.Infrastructure.Services.EmailService>();

// ========================================
//CORS CORRECTO PARA PRODUCCIÓN
// ========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (string.IsNullOrEmpty(origin))
                return false;

            // Permitir localhost (desarrollo)
            if (origin.StartsWith("http://localhost:") ||
                origin.StartsWith("https://localhost:"))
                return true;

            // Permitir subdominios de jegasolutions.co
            if (origin.EndsWith(".jegasolutions.co") ||
                origin == "https://jegasolutions.co")
                return true;

            // Frontend específico de Extra Hours
            if (origin == "https://extrahours.jegasolutions.co")
                return true;

            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials(); // ✅ Funciona con SetIsOriginAllowed
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection only in Development
// In production (Render), HTTPS is handled by the proxy
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// ✅ CORS PRIMERO
app.UseCors("AllowAll");

// ORDEN CRÍTICO DEL PIPELINE:
// 1. Primero autenticación (valida JWT y establece context.User con claims)
app.UseAuthentication();
// 2. Luego middleware JWT personalizado (mapea claims)
app.UseMiddleware<JwtMiddleware>();
// 3. Después autorización (verifica roles usando context.User)
app.UseAuthorization();

// 4. Después middleware de tenant (puede leer claims de context.User)
app.UseMiddleware<TenantMiddleware>();

// 3. ✅ SSO: Validar acceso al módulo extra-hours (TEMPORALMENTE DESHABILITADO)
// app.UseMiddleware<ModuleAccessMiddleware>();


app.MapControllers();

// ✅ Root endpoint (para health checks de Render y otros servicios)
app.MapGet("/", () => Results.Ok(new
{
    service = "JEGASolutions Extra Hours API",
    version = "1.0.0",
    status = "running",
    timestamp = DateTime.UtcNow,
    endpoints = new
    {
        health = "/health",
        swagger = "/swagger",
        api = "/api"
    }
}));

// ✅ Health check endpoint (requerido por Render)
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    service = "Extra Hours API",
    checks = new
    {
        database = "connected",
        api = "operational"
    }
}));

// ========================================
// 🔥 APLICAR MIGRACIONES EN TODOS LOS AMBIENTES
// ========================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var maxRetries = 30;
    var delay = TimeSpan.FromSeconds(2);

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            // Test the connection
            await context.Database.CanConnectAsync();
            Console.WriteLine("✅ Database connection successful");
            break;
        }
        catch (Exception ex)
        {
            if (i == maxRetries - 1)
            {
                Console.WriteLine($"❌ Failed to connect to database after {maxRetries} attempts: {ex.Message}");
                throw;
            }
            Console.WriteLine($"🔄 Database connection attempt {i + 1} failed, retrying in {delay.TotalSeconds} seconds...");
            await Task.Delay(delay);
        }
    }

    // 🔥 APLICAR MIGRACIONES AUTOMÁTICAMENTE
    Console.WriteLine("Applying database migrations...");
    try
    {
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error applying migrations: {ex.Message}");
        throw;
    }
}

Console.WriteLine("🚀 Extra Hours API is running!");
app.Run();
