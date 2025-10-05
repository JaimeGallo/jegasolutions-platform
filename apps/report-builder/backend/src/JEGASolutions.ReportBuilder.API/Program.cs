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

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        Environment.GetEnvironmentVariable("DB_HOST") != null &&
        Environment.GetEnvironmentVariable("DB_NAME") != null &&
        Environment.GetEnvironmentVariable("DB_USER") != null &&
        Environment.GetEnvironmentVariable("DB_PASSWORD") != null
            ? $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Database={Environment.GetEnvironmentVariable("DB_NAME")};Username={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}"
            : builder.Configuration.GetConnectionString("DefaultConnection")
    )
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
                 builder.Configuration["OpenAI:ApiKey"];
    return new OpenAIClient(apiKey);
});

// Register HttpClient for AI services
builder.Services.AddHttpClient();

// Register AI providers
builder.Services.AddScoped<IAIProvider, OpenAIProviderService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<OpenAIProviderService>>();
    return new OpenAIProviderService(httpClient, config, logger);
});

builder.Services.AddScoped<IAIProvider, AnthropicService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<AnthropicService>>();
    return new AnthropicService(httpClient, config, logger);
});

builder.Services.AddScoped<IAIProvider, DeepSeekService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<DeepSeekService>>();
    return new DeepSeekService(httpClient, config, logger);
});

builder.Services.AddScoped<IAIProvider, GroqService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<GroqService>>();
    return new GroqService(httpClient, config, logger);
});

// Register MultiAI coordinator service
builder.Services.AddScoped<IMultiAIService, MultiAIService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? 
                 builder.Configuration["JwtSettings:SecretKey"];
    
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

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (string.IsNullOrWhiteSpace(origin)) return false;
            var uri = new Uri(origin);
            return uri.Host == "localhost" || uri.Host == "127.0.0.1";
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
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

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make Program class accessible for testing
public partial class Program { }