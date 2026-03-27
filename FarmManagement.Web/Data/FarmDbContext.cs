using Microsoft.EntityFrameworkCore;
using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Data
{
    public class FarmDbContext : DbContext
    {
        public FarmDbContext(DbContextOptions<FarmDbContext> options) : base(options) { }

        // ── Member 1 Tables ──────────────────────────────────────────
        public DbSet<Crop> Crops { get; set; }
        public DbSet<Field> Fields { get; set; }

        // ── Member 2 Tables ──────────────────────────────────────────
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceUsage> ResourceUsage { get; set; }

        // ── Member 3 Tables ──────────────────────────────────────────
        public DbSet<PlantingSchedule> PlantingSchedules { get; set; }
        public DbSet<Harvest> Harvests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Resource → ResourceUsage (One-to-Many) ───────────────
            modelBuilder.Entity<ResourceUsage>()
                .HasOne(u => u.Resource)
                .WithMany(r => r.Usages)
                .HasForeignKey(u => u.ResourceId)
                .OnDelete(DeleteBehavior.Restrict); // don't cascade-delete usage logs

            // ── Field → ResourceUsage (optional FK) ─────────────────
            modelBuilder.Entity<ResourceUsage>()
                .HasOne(u => u.Field)
                .WithMany()
                .HasForeignKey(u => u.FieldId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // ── Resource: QuantityAvailable precision ────────────────
            modelBuilder.Entity<Resource>()
                .Property(r => r.QuantityAvailable)
                .HasColumnType("decimal(18,4)");

            // ── ResourceUsage: QuantityUsed precision ────────────────
            modelBuilder.Entity<ResourceUsage>()
                .Property(u => u.QuantityUsed)
                .HasColumnType("decimal(18,4)");
        }
    }
}