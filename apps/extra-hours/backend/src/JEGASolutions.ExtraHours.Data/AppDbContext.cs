using Microsoft.EntityFrameworkCore;
using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> employees { get; set; }
        public DbSet<ExtraHour> extraHours { get; set; }
        public DbSet<ExtraHoursConfig> extraHoursConfigs { get; set; }
        public DbSet<Manager> managers { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<CompensationRequest> compensationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure multi-tenant properties for all entities
            ConfigureMultiTenantProperties(modelBuilder);

            // Relación entre Employee y Manager
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.manager)
                .WithMany()
                .HasForeignKey(e => e.manager_id)
                .OnDelete(DeleteBehavior.SetNull);

            // Relación entre Manager y User
            modelBuilder.Entity<Manager>()
                .HasOne(m => m.User)
                .WithOne()
                .HasForeignKey<Manager>(m => m.manager_id)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre Employee y User
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employee>(e => e.id)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre ExtraHour y Employee
            modelBuilder.Entity<ExtraHour>()
                .HasOne(eh => eh.employee)
                .WithMany()
                .HasForeignKey(eh => eh.id)  // ← Cambiar aquí
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre CompensationRequest y Employee
            modelBuilder.Entity<CompensationRequest>()
                .HasOne(cr => cr.Employee)
                .WithMany()
                .HasForeignKey(cr => cr.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre CompensationRequest y User (ApprovedBy)
            modelBuilder.Entity<CompensationRequest>()
                .HasOne(cr => cr.ApprovedBy)
                .WithMany()
                .HasForeignKey(cr => cr.ApprovedById)
                .OnDelete(DeleteBehavior.SetNull);

            // ✅ Configuración explícita de tabla CompensationRequest
            modelBuilder.Entity<CompensationRequest>()
                .ToTable("compensationrequests"); // Sin guión bajo para coincidir con la BD de producción

            // Datos iniciales para ExtraHoursConfig
            modelBuilder.Entity<ExtraHoursConfig>().HasData(new ExtraHoursConfig { id = 1 });

            // ✅ Conversión automática de audit fields a UTC para ExtraHoursConfig
            modelBuilder.Entity<ExtraHoursConfig>()
                .Property(e => e.CreatedAt)
                .HasConversion(
                    v => v.HasValue ? (v.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                        : v.Value.ToUniversalTime()) : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);

            modelBuilder.Entity<ExtraHoursConfig>()
                .Property(e => e.UpdatedAt)
                .HasConversion(
                    v => v.HasValue ? (v.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                        : v.Value.ToUniversalTime()) : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);

            modelBuilder.Entity<ExtraHoursConfig>()
                .Property(e => e.DeletedAt)
                .HasConversion(
                    v => v.HasValue ? (v.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                        : v.Value.ToUniversalTime()) : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);

            // Configuración de fechas en UTC
            modelBuilder.Entity<ExtraHour>()
                .Property(e => e.date)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<CompensationRequest>()
                .Property(cr => cr.WorkDate)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<CompensationRequest>()
                .Property(cr => cr.RequestedCompensationDate)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<CompensationRequest>()
                .Property(cr => cr.RequestedAt)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<CompensationRequest>()
                .Property(cr => cr.DecidedAt)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);

            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureMultiTenantProperties(ModelBuilder modelBuilder)
        {
            // Configure TenantId for all entities
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_employees_tenant_id");

            modelBuilder.Entity<ExtraHour>()
                .HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_extra_hours_tenant_id");

            modelBuilder.Entity<ExtraHoursConfig>()
                .HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_extra_hours_config_tenant_id");

            modelBuilder.Entity<Manager>()
                .HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_managers_tenant_id");

            modelBuilder.Entity<User>()
                .HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_users_tenant_id");

            modelBuilder.Entity<CompensationRequest>()
                .HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_compensation_requests_tenant_id");

            // Configure TenantId as optional with default value for backwards compatibility
            modelBuilder.Entity<Employee>()
                .Property(e => e.TenantId)
                .HasDefaultValue(1)
                .IsRequired(false);

            modelBuilder.Entity<ExtraHour>()
                .Property(e => e.TenantId)
                .HasDefaultValue(1)
                .IsRequired(false);

            modelBuilder.Entity<ExtraHoursConfig>()
                .Property(e => e.TenantId)
                .HasDefaultValue(1)
                .IsRequired(false);

            modelBuilder.Entity<Manager>()
                .Property(e => e.TenantId)
                .HasDefaultValue(1)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .Property(e => e.TenantId)
                .HasDefaultValue(1)
                .IsRequired(false);

            modelBuilder.Entity<CompensationRequest>()
                .Property(e => e.TenantId)
                .HasDefaultValue(1)
                .IsRequired(false);

            // Configure audit fields
            ConfigureAuditFields(modelBuilder);
        }

        private void ConfigureAuditFields(ModelBuilder modelBuilder)
        {
            // Configure CreatedAt, UpdatedAt, DeletedAt for all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(JEGASolutions.ExtraHours.Core.Entities.TenantEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Configure audit fields with default values
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("CreatedAt")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
                        .IsRequired(false);

                    modelBuilder.Entity(entityType.ClrType)
                        .Property("UpdatedAt")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
                        .IsRequired(false);

                    modelBuilder.Entity(entityType.ClrType)
                        .Property("DeletedAt")
                        .IsRequired(false);
                }
            }
        }
    }
}
