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

// Configuration
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Database
// 🔹 Si Render define DATABASE_URL, conviértela al formato Npgsql
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var host = uri.Host;
        var port = uri.Port != -1 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : string.Empty;

        var npgsqlConnectionString =
            $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";

        builder.Configuration["ConnectionStrings:DefaultConnection"] = npgsqlConnectionString;

        Console.WriteLine($"✅ DATABASE_URL detected and parsed for host: {host}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Failed to parse DATABASE_URL: {ex.Message}");
    }
}
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://jegasolutions-platform-frontend.vercel.app")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});



var app = builder.Build();

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

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Endpoint básico para testing
app.MapGet("/", () => new
{
    message = "JEGASolutions Backend is working!",
    version = "v4",
    timestamp = DateTime.UtcNow
});

app.MapGet("/health", () => new { status = "OK" });

Console.WriteLine("=== BACKEND READY V4 ===");
app.Run();

