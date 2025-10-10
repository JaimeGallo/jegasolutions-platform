using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using JEGASolutions.ExtraHours.Data;
using JEGASolutions.ExtraHours.Core.Interfaces;
using JEGASolutions.ExtraHours.Core.Services;
using JEGASolutions.ExtraHours.Infrastructure.Repositories;
using JEGASolutions.ExtraHours.Infrastructure.Services;
using JEGASolutions.ExtraHours.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            // ✅ MAPEO DE CLAIMS
            RoleClaimType = "role",
            NameClaimType = ClaimTypes.Name
        };
    });

builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<ITenantContextService, TenantContextService>();
builder.Services.AddScoped<IJWTUtils, JEGASolutions.ExtraHours.Infrastructure.Services.JWTUtils>();
builder.Services.AddScoped<IAuthService, JEGASolutions.ExtraHours.Infrastructure.Services.AuthService>();
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
// Re-enabled services for full functionality
builder.Services.AddScoped<IExtraHourCalculationService, ExtraHourCalculationService>();
builder.Services.AddScoped<ICompensationRequestService, CompensationRequestService>();
builder.Services.AddScoped<IEmailService, JEGASolutions.ExtraHours.Infrastructure.Services.EmailService>();

// ========================================
// 🔧 FIX: CORS CORRECTO PARA PRODUCCIÓN
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
        .AllowCredentials(); // ✅ IMPORTANTE: Ahora funciona con SetIsOriginAllowed
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ CORS PRIMERO, antes de Authentication
app.UseCors("AllowAll");

// ORDEN CRÍTICO DEL PIPELINE:
// 1. Primero autenticación (valida JWT y establece context.User con claims)
app.UseAuthentication();
// 2. Luego autorización (verifica roles usando context.User)
app.UseAuthorization();

// 3. Finalmente middleware de tenant (puede leer claims de context.User)
app.UseMiddleware<TenantMiddleware>();

app.MapControllers();

// Wait for database to be ready (health check) but DO NOT create tables
// The tables will be created from the SQL backup script
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var maxRetries = 30;
    var delay = TimeSpan.FromSeconds(2);

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            // Just test the connection, don't create database
            await context.Database.CanConnectAsync();
            Console.WriteLine("Database connection successful");
            break;
        }
        catch (Exception ex)
        {
            if (i == maxRetries - 1)
            {
                Console.WriteLine($"Failed to connect to database after {maxRetries} attempts: {ex.Message}");
                throw;
            }
            Console.WriteLine($"Database connection attempt {i + 1} failed, retrying in {delay.TotalSeconds} seconds...");
            await Task.Delay(delay);
        }
    }
}

app.Run();
