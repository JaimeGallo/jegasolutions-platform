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

var builder = WebApplication.CreateBuilder(args);

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
);

// JWT Authentication

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero,

        // ✅ MAPEO DE CLAIMS CORTOS
        RoleClaimType = "role",
        NameClaimType = "name"
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            
            if (context.Principal?.Identity is ClaimsIdentity identity)
            {
                logger.LogInformation("✅ JWT Token validated successfully");
                
                var allClaims = identity.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                logger.LogInformation($"🔍 ALL claims: {string.Join(", ", allClaims)}");
                
                // Verificar claim de rol CON AMBOS NOMBRES
                var roleShort = identity.FindFirst("role");
                var roleLong = identity.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                
                if (roleShort != null)
                {
                    logger.LogInformation($"✅✅✅ Role (SHORT) found: {roleShort.Value}");
                }
                else if (roleLong != null)
                {
                    logger.LogWarning($"⚠️⚠️⚠️ Role found but with LONG URI: {roleLong.Value}");
                    logger.LogWarning("⚠️ This means the token was generated BEFORE updating Landing API!");
                    logger.LogWarning("⚠️ YOU MUST LOGOUT AND LOGIN AGAIN to get a new token!");
                }
                else
                {
                    logger.LogError("❌ NO role claim found at all!");
                }
                
                // Verificar IsInRole
                logger.LogInformation($"🔍 User.IsInRole('superusuario'): {context.HttpContext.User.IsInRole("superusuario")}");
                logger.LogInformation($"🔍 User.IsInRole('manager'): {context.HttpContext.User.IsInRole("manager")}");
            }
            
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();
            logger.LogError($"❌ Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();
            logger.LogWarning($"⚠️ Authentication challenge: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var user = context.HttpContext.User;
            
            var rolesShort = user.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
            var rolesLong = user.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value).ToList();
            
            logger.LogWarning($"🚫 FORBIDDEN - Endpoint: {context.HttpContext.Request.Path}");
            logger.LogWarning($"🚫 Roles (short): {string.Join(", ", rolesShort)} - Count: {rolesShort.Count}");
            logger.LogWarning($"🚫 Roles (long): {string.Join(", ", rolesLong)} - Count: {rolesLong.Count}");
            
            if (rolesLong.Any() && !rolesShort.Any())
            {
                logger.LogError("❌❌❌ TOKEN IS OLD! User must LOGOUT and LOGIN again!");
            }
            
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<ITenantContextService, TenantContextService>();
// ❌ SSO: Ya no se usa AuthService ni JWTUtils locales, el Landing maneja la autenticación
// builder.Services.AddScoped<IJWTUtils, JEGASolutions.ExtraHours.Infrastructure.Services.JWTUtils>();
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

// 4. Finalmente middleware de tenant (puede leer claims de context.User)
app.UseMiddleware<TenantMiddleware>();

// ORDEN CRÍTICO DEL PIPELINE:
// 1. Primero autenticación (valida JWT y establece context.User con claims)
app.UseAuthentication();
// 2. Luego autorización (verifica roles usando context.User)
app.UseAuthorization();

// 3. ✅ SSO: Validar acceso al módulo extra-hours (TEMPORALMENTE DESHABILITADO)
// app.UseMiddleware<ModuleAccessMiddleware>();


app.MapControllers();

// ✅ Health check endpoint (requerido por Render)
app.MapGet("/health", () => new
{
    status = "OK",
    timestamp = DateTime.UtcNow,
    service = "Extra Hours API"
});

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
