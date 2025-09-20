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

            // Datos iniciales para ExtraHoursConfig
            modelBuilder.Entity<ExtraHoursConfig>().HasData(new ExtraHoursConfig { id = 1 });

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
    }
}