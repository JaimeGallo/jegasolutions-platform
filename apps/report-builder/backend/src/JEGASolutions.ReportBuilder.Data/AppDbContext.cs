using Microsoft.EntityFrameworkCore;
using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Template> Templates { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<ReportSubmission> ReportSubmissions { get; set; }
        public DbSet<AIInsight> AIInsights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure multi-tenant filtering
            ConfigureTenantFiltering(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Template>()
                .HasOne(t => t.Area)
                .WithMany()
                .HasForeignKey(t => t.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReportSubmission>()
                .HasOne(rs => rs.Template)
                .WithMany()
                .HasForeignKey(rs => rs.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReportSubmission>()
                .HasOne(rs => rs.Area)
                .WithMany()
                .HasForeignKey(rs => rs.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AIInsight>()
                .HasOne(ai => ai.ReportSubmission)
                .WithMany()
                .HasForeignKey(ai => ai.ReportSubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes for performance
            modelBuilder.Entity<Template>()
                .HasIndex(t => new { t.TenantId, t.AreaId })
                .HasDatabaseName("IX_Templates_TenantId_AreaId");

            modelBuilder.Entity<ReportSubmission>()
                .HasIndex(rs => new { rs.TenantId, rs.Status })
                .HasDatabaseName("IX_ReportSubmissions_TenantId_Status");

            modelBuilder.Entity<AIInsight>()
                .HasIndex(ai => new { ai.TenantId, ai.InsightType })
                .HasDatabaseName("IX_AIInsights_TenantId_InsightType");
        }

        private void ConfigureTenantFiltering(ModelBuilder modelBuilder)
        {
            // Configure global query filters for multi-tenancy
            modelBuilder.Entity<Template>()
                .HasQueryFilter(t => !t.IsDeleted);

            modelBuilder.Entity<Area>()
                .HasQueryFilter(a => !a.IsDeleted);

            modelBuilder.Entity<ReportSubmission>()
                .HasQueryFilter(rs => !rs.IsDeleted);

            modelBuilder.Entity<AIInsight>()
                .HasQueryFilter(ai => !ai.IsDeleted);
        }
    }
}
