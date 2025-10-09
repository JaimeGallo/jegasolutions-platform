using System.Reflection;
using System.Text;
using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Application.Services;
using JEGASolutions.Landing.Domain.Interfaces;
using JEGASolutions.Landing.Application.Configuration;
using JEGASolutions.Landing.Infrastructure.Data;
using JEGASolutions.Landing.Infrastructure.Repositories;
using JEGASolutions.Landing.Infrastructure.Services;
using JEGASolutions.Landing.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Debug log para verificar rebuild
Console.WriteLine("=== BACKEND STARTING V4 ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Database
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    try
    {
        // Ejemplo de DATABASE_URL:
        // postgresql://usuario:contraseña@host:5432/base?sslmode=require
        var cleanedUrl = databaseUrl.Replace("postgres://", "postgresql://"); // compatibilidad
        var uri = new Uri(cleanedUrl);

        var userInfo = uri.UserInfo.Split(':');
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');

        // ⚡ Procesar parámetros de la query string (?sslmode=require&foo=bar)
        var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var sslMode = queryParams["sslmode"] ?? "Require";

        var npgsqlConnectionString =
            $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode};Trust Server Certificate=true";

        builder.Configuration["ConnectionStrings:DefaultConnection"] = npgsqlConnectionString;

        Console.WriteLine($"✅ DATABASE_URL parsed successfully for host: {host}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Failed to parse DATABASE_URL: {ex.Message}");
    }
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// ===== CONFIGURAR SMTP SETTINGS =====
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Email"));

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IWompiService, WompiService>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("JEGASolutions.Landing.Application")));

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
    };
});


// HTTP Clients
builder.Services.AddHttpClient<IWompiService, WompiService>();

// CORS - ACTUALIZADO para permitir todos los deployments de Vercel + Tenant Dashboard Wildcard
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (string.IsNullOrEmpty(origin))
                return false;

            // Permitir localhost (desarrollo)
            if (origin.StartsWith("http://localhost:") || origin.StartsWith("https://localhost:"))
                return true;

            // Permitir TODOS los deployments de Vercel
            if (origin.Contains(".vercel.app"))
                return true;

            // Permitir dominio principal
            if (origin == "https://jegasolutions.co" || origin == "https://www.jegasolutions.co")
                return true;

            // ✅ NUEVO: Permitir TODOS los subdominios de jegasolutions.co (Tenant Dashboard)
            // Ejemplos: https://cliente-a.jegasolutions.co, https://techcorp.jegasolutions.co
            var uri = new Uri(origin);
            if (uri.Host.EndsWith(".jegasolutions.co"))
                return true;

            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});


var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

// Ejecutar migraciones automáticamente en startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error applying migrations: {ex.Message}");
    }
}

// swagger
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===== MIDDLEWARE PIPELINE =====

// 1. HTTPS Redirection (antes de CORS)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// 2. CORS (debe estar temprano en el pipeline)
app.UseCors("AllowFrontend");

// 3. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 4. Map Controllers
app.MapControllers();

// ===== HEALTH CHECK ENDPOINTS =====
app.MapGet("/", () => new
{
    message = "JEGASolutions Backend is working!",
    version = "v5-production-ready",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
});

app.MapGet("/health", () => new
{
    status = "OK",
    timestamp = DateTime.UtcNow
});

// ===== ENDPOINT DE PRUEBA DE EMAIL =====
app.MapGet("/test-email", async (IServiceProvider serviceProvider) =>
{
    try
    {
        var emailService = serviceProvider.GetRequiredService<IEmailService>();

        var htmlBody = @"<div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h1 style='color: #2563eb;'>¡Funciona!</h1>
                <p>El SMTP de Microsoft 365 está configurado correctamente.</p>
                <p>Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + @"</p>
              </div>";

        var result = await emailService.SendWelcomeEmailAsync(
            "JaimeGallo@jegasolutions.co",
            "🧪 Test SMTP Microsoft 365",
            htmlBody
        );

        if (result)
        {
            return Results.Ok(new {
                success = true,
                message = "✅ Email enviado correctamente a JaimeGallo@jegasolutions.co"
            });
        }
        else
        {
            return Results.BadRequest(new {
                success = false,
                error = "No se pudo enviar el email (método devolvió false)"
            });
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new {
            success = false,
            error = ex.Message,
            details = ex.InnerException?.Message
        });
    }
});

// ===== START SERVER =====
Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║  JEGASOLUTIONS BACKEND V5 - READY! 🚀     ║");
Console.WriteLine("║  Environment: " + app.Environment.EnvironmentName.PadRight(27) + " ║");
Console.WriteLine("║  Tenant Dashboard: ENABLED ✅              ║");
Console.WriteLine("╚════════════════════════════════════════════╝");

app.Run();

