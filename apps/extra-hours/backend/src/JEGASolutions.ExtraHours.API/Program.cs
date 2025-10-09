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
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
                    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is not configured");
}

Console.WriteLine($"✅ Database connection configured: {connectionString.Substring(0, Math.Min(30, connectionString.Length))}...");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

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

// Add CORS
// Add CORS - CONFIGURACIÓN ACTUALIZADA
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (string.IsNullOrEmpty(origin))
                return false;

            // Permitir localhost
            if (origin.StartsWith("http://localhost:") || origin.StartsWith("https://localhost:"))
                return true;

            // Permitir todos los dominios de Vercel
            if (origin.Contains(".vercel.app"))
                return true;

            // Permitir dominios personalizados
            if (origin.Contains("jegasolutions.co"))
                return true;

            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ⚠️ ORDEN CRÍTICO DEL PIPELINE (muy importante para CORS):
// 1. HTTPS Redirection
app.UseHttpsRedirection();

// 2. Routing (NECESARIO para que CORS funcione con endpoints)
app.UseRouting();

// 3. CORS (debe ir DESPUÉS de UseRouting y ANTES de UseAuthentication)
app.UseCors("AllowAll");

// 4. Authentication (valida JWT y establece context.User con claims)
app.UseAuthentication();

// 5. Authorization (verifica roles usando context.User)
app.UseAuthorization();

// 6. Middleware personalizado de tenant (puede leer claims de context.User)
app.UseMiddleware<TenantMiddleware>();

// 7. Map Controllers
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
            Console.WriteLine($"⏳ Database connection attempt {i + 1}/{maxRetries} failed, retrying in {delay.TotalSeconds} seconds...");
            await Task.Delay(delay);
        }
    }
}

app.Run();
