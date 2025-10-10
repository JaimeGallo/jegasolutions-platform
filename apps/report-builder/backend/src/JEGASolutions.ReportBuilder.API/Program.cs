using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JEGASolutions.ReportBuilder.Data;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Core.Services;
using JEGASolutions.ReportBuilder.Infrastructure.Repositories;
using JEGASolutions.ReportBuilder.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========================================
// DATABASE CONFIGURATION - Snake Case
// ========================================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MapToSnakeCase();
    });
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IReportSubmissionRepository, ReportSubmissionRepository>();
builder.Services.AddScoped<IAiInsightRepository, AiInsightRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IOpenAiService, OpenAiService>();

// ========================================
// JWT AUTHENTICATION
// ========================================
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ??
             builder.Configuration["JwtSettings:SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                      builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
                        builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT secret key is required"))
        ),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ========================================
// CORS - FIXED FOR PRODUCTION
// ========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (string.IsNullOrWhiteSpace(origin)) return false;

            var uri = new Uri(origin);

            // Permitir localhost (desarrollo)
            if (uri.Host == "localhost" || uri.Host == "127.0.0.1")
                return true;

            // Permitir dominios de producción
            if (uri.Host.EndsWith(".jegasolutions.co"))
                return true;

            // Permitir Vercel deployments
            if (uri.Host.EndsWith(".vercel.app"))
                return true;

            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// ========================================
// MIGRATIONS - Development Only
// ========================================
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Checking database connection...");

    var retryCount = 0;
    var maxRetries = 10;
    var delayMs = 2000;

    while (retryCount < maxRetries)
    {
        try
        {
            if (dbContext.Database.CanConnect())
            {
                logger.LogInformation("Database connection successful");
                break;
            }
        }
        catch (Exception ex)
        {
            retryCount++;
            logger.LogWarning(
                "Database connection attempt {Retry}/{MaxRetries} failed: {Error}",
                retryCount, maxRetries, ex.Message
            );

            if (retryCount >= maxRetries)
            {
                logger.LogError("Failed to connect to database after {MaxRetries} attempts", maxRetries);
                throw;
            }

            await Task.Delay(delayMs);
        }
    }

    logger.LogInformation("Applying database migrations...");
    try
    {
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error applying database migrations");
        throw;
    }
}

// ========================================
// MIDDLEWARE PIPELINE
// ========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CRITICAL: CORS must be before Authentication/Authorization
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new
{
    status = "OK",
    timestamp = DateTime.UtcNow,
    service = "ReportBuilder API"
});

app.Run();

public partial class Program { }
