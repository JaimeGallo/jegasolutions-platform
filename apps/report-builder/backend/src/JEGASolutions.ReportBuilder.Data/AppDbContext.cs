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
        public DbSet<User> Users { get; set; }
        public DbSet<ConsolidatedTemplate> ConsolidatedTemplates { get; set; }
        public DbSet<ConsolidatedTemplateSection> ConsolidatedTemplateSections { get; set; }
        public DbSet<ExcelUpload> ExcelUploads { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // CRITICAL: Enable snake_case naming convention
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSnakeCaseNamingConvention();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure multi-tenant filtering
            ConfigureTenantFiltering(modelBuilder);

            // Configure User entity
            ConfigureUserEntity(modelBuilder);

            // Configure Template entity with defaults
            ConfigureTemplateEntity(modelBuilder);

            // Configure Area entity with defaults
            ConfigureAreaEntity(modelBuilder);

            // Configure ReportSubmission entity with defaults
            ConfigureReportSubmissionEntity(modelBuilder);

            // Configure AIInsight entity with defaults
            ConfigureAIInsightEntity(modelBuilder);

            // Configure ConsolidatedTemplate entity with defaults
            ConfigureConsolidatedTemplateEntity(modelBuilder);

            // Configure ConsolidatedTemplateSection entity with defaults
            ConfigureConsolidatedTemplateSectionEntity(modelBuilder);

            // Configure ExcelUpload entity with defaults
            ConfigureExcelUploadEntity(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Template>()
                .HasOne(t => t.Area)
                .WithMany()
                .HasForeignKey(t => t.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReportSubmission>()
                .HasOne(r => r.Template)
                .WithMany()
                .HasForeignKey(r => r.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AIInsight>()
                .HasOne(ai => ai.ReportSubmission)
                .WithMany()
                .HasForeignKey(ai => ai.ReportSubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConsolidatedTemplateSection>()
                .HasOne(cts => cts.ConsolidatedTemplate)
                .WithMany(ct => ct.Sections)
                .HasForeignKey(cts => cts.ConsolidatedTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConsolidatedTemplateSection>()
                .HasOne(cts => cts.Area)
                .WithMany()
                .HasForeignKey(cts => cts.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExcelUpload>()
                .HasOne(eu => eu.Area)
                .WithMany()
                .HasForeignKey(eu => eu.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureTenantFiltering(ModelBuilder modelBuilder)
        {
            // Apply soft delete filter to all entities
            modelBuilder.Entity<Template>()
                .HasQueryFilter(t => t.DeletedAt == null);

            modelBuilder.Entity<Area>()
                .HasQueryFilter(a => a.DeletedAt == null);

            modelBuilder.Entity<ReportSubmission>()
                .HasQueryFilter(r => r.DeletedAt == null);

            modelBuilder.Entity<AIInsight>()
                .HasQueryFilter(ai => ai.DeletedAt == null);

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => u.DeletedAt == null);

            modelBuilder.Entity<ConsolidatedTemplate>()
                .HasQueryFilter(ct => ct.DeletedAt == null);

            modelBuilder.Entity<ConsolidatedTemplateSection>()
                .HasQueryFilter(cts => cts.DeletedAt == null);

            modelBuilder.Entity<ExcelUpload>()
                .HasQueryFilter(eu => eu.DeletedAt == null);
        }

        private void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Unique email constraint
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");

                // Index for tenant queries
                entity.HasIndex(e => new { e.TenantId, e.IsActive })
                    .HasDatabaseName("IX_Users_TenantId_IsActive");

                // Required fields
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(255);
                entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("User");

                // Default values for audit columns
                entity.Property(e => e.TenantId).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });
        }

        private void ConfigureTemplateEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Template>(entity =>
            {
                // Default values for audit columns
                entity.Property(e => e.TenantId).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
            });
        }

        private void ConfigureAreaEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Area>(entity =>
            {
                // Default values for audit columns
                entity.Property(e => e.TenantId).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });
        }

        private void ConfigureReportSubmissionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportSubmission>(entity =>
            {
                // Default values for audit columns
                entity.Property(e => e.TenantId).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.Status).HasDefaultValue("draft");
            });
        }

        private void ConfigureAIInsightEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AIInsight>(entity =>
            {
                // Default values for audit columns
                entity.Property(e => e.TenantId).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.GeneratedAt).HasDefaultValueSql("NOW()");
            });
        }

        private void ConfigureConsolidatedTemplateEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConsolidatedTemplate>(entity =>
            {
                // Primary key
                entity.HasKey(e => e.Id);

                // Required fields
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("draft");
                entity.Property(e => e.Period).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedByUserId).IsRequired();

                // Default values for audit columns
                entity.Property(e => e.TenantId).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
            });
        }

        private void ConfigureConsolidatedTemplateSectionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConsolidatedTemplateSection>(entity =>
            {
                // Primary key
                entity.HasKey(e => e.Id);

                // Required fields
                entity.Property(e => e.ConsolidatedTemplateId).IsRequired();
                entity.Property(e => e.AreaId).IsRequired();
                entity.Property(e => e.SectionTitle).IsRequired().HasMaxLength(200);
                entity.Property(e => e.SectionDescription).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("pending");
                entity.Property(e => e.Order).HasDefaultValue(1);

                // Default values for audit columns
                entity.Property(e => e.TenantId).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
            });
        }

        private void ConfigureExcelUploadEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExcelUpload>(entity =>
            {
                // Primary key
                entity.HasKey(e => e.Id);

                // Required fields
                entity.Property(e => e.AreaId).IsRequired();
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Period).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UploadedByUserId).IsRequired();
                entity.Property(e => e.ProcessingStatus).IsRequired().HasMaxLength(20).HasDefaultValue("pending");

                // Default values for audit columns
                entity.Property(e => e.TenantId).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UploadDate).HasDefaultValueSql("NOW()");
            });
        }
    }
}
