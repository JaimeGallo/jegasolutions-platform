using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JEGASolutions.ReportBuilder.Data;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Infrastructure.Repositories;
using JEGASolutions.ReportBuilder.Core.Services;
using JEGASolutions.ReportBuilder.Infrastructure.Services;
using JEGASolutions.ReportBuilder.Infrastructure.Services.AI;
using Azure.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========================================
// 🔧 Configure DbContext with Snake Case Naming
// ========================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        Environment.GetEnvironmentVariable("DB_HOST") != null &&
        Environment.GetEnvironmentVariable("DB_NAME") != null &&
        Environment.GetEnvironmentVariable("DB_USER") != null &&
        Environment.GetEnvironmentVariable("DB_PASSWORD") != null
            ? $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Database={Environment.GetEnvironmentVariable("DB_NAME")};Username={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}"
            : builder.Configuration.GetConnectionString("DefaultConnection")
    )
    .UseSnakeCaseNamingConvention() // ✅ AGREGADO: Snake case naming
);

// Register repositories
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IReportSubmissionRepository, ReportSubmissionRepository>();
builder.Services.AddScoped<IUserRepository, JEGASolutions.ReportBuilder.Infrastructure.Repositories.UserRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IConsolidatedTemplateRepository, ConsolidatedTemplateRepository>();
builder.Services.AddScoped<IConsolidatedTemplateSectionRepository, ConsolidatedTemplateSectionRepository>();
builder.Services.AddScoped<IExcelUploadRepository, ExcelUploadRepository>();

// Register services
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IReportSubmissionService, ReportSubmissionService>();
builder.Services.AddScoped<IAIAnalysisService, OpenAIService>();
builder.Services.AddScoped<IAuthService, JEGASolutions.ReportBuilder.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<IConsolidatedTemplateService, ConsolidatedTemplateService>();
builder.Services.AddScoped<IExcelProcessorService, ExcelProcessorService>();

// Register OpenAI client
builder.Services.AddSingleton<OpenAIClient>(provider =>
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                 builder.Configuration["OpenAI:ApiKey"] ??
                 "placeholder";
    return new OpenAIClient(apiKey, new OpenAIClientOptions());
});

// JWT Authentication
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ??
             builder.Configuration["JwtSettings:SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                          builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
                            builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT secret key is required"))),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ========================================
// 🔧 Configure CORS for Production
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
            if (uri.Host.EndsWith(".jegasolutions.co") || uri.Host == "jegasolutions.co")
                return true;

            // Frontend específico de Report Builder
            if (origin == "https://reportbuilder.jegasolutions.co")
                return true;

            return false;
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // ✅ Funciona con SetIsOriginAllowed
    });
});

var app = builder.Build();

// Apply migrations automatically in Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Checking database connection...");

    // Wait for database to be ready (retry logic)
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
            logger.LogWarning("Database connection attempt {Retry}/{MaxRetries} failed: {Error}",
                retryCount, maxRetries, ex.Message);

            if (retryCount >= maxRetries)
            {
                logger.LogError("Failed to connect to database after {MaxRetries} attempts", maxRetries);
                throw;
            }

            await Task.Delay(delayMs);
        }
    }

    // Apply pending migrations
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ CORS PRIMERO
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🚀 Report Builder API is running!");
app.Run();

// Make Program class accessible for testing
public partial class Program { }
