using Microsoft.EntityFrameworkCore;
using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<ExtraHour> ExtraHours { get; set; }
        public DbSet<ExtraHoursConfig> ExtraHoursConfigs { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CompensationRequest> CompensationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación entre Employee y Manager
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relación entre Manager y User
            modelBuilder.Entity<Manager>()
                .HasOne(m => m.User)
                .WithOne()
                .HasForeignKey<Manager>(m => m.ManagerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre Employee y User
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employee>(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre ExtraHour y Employee
            modelBuilder.Entity<ExtraHour>()
                .HasOne(eh => eh.Employee)
                .WithMany()
                .HasForeignKey(eh => eh.EmployeeId)
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
            modelBuilder.Entity<ExtraHoursConfig>().HasData(new ExtraHoursConfig { Id = 1 });

            // Configuración de fechas en UTC
            modelBuilder.Entity<ExtraHour>()
                .Property(e => e.Date)
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
