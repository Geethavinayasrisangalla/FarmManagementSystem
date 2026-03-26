using FarmManagement.Web.Models;
using FarmManagement.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Data
{
    public class PestDbContext : DbContext
    {
        public PestDbContext(DbContextOptions<PestDbContext> options) : base(options)
        {
        }

        public DbSet<PestIncident> PestIncidents { get; set; }
        public DbSet<Treatment> Treatments { get; set; }

        public DbSet<Field> Fields { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<Resource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Field>().ToTable("Fields", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Crop>().ToTable("Crops", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Resource>().ToTable("Resources", t => t.ExcludeFromMigrations());
        }
    }
}

