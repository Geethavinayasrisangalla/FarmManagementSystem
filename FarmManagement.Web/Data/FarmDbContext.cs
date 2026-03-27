using Microsoft.EntityFrameworkCore;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models;

namespace FarmManagement.Web.Data
{
    public class FarmDbContext : DbContext
    {
        public FarmDbContext(DbContextOptions<FarmDbContext> options) : base(options) { }

        // Member 1 Tables
        public DbSet<Crop> Crops { get; set; }
        public DbSet<Field> Fields { get; set; }

        // Member 2 Tables
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceUsage> ResourceUsage { get; set; }

        // Member 3 Tables (ADD THESE NOW)
        public DbSet<PlantingSchedule> PlantingSchedules { get; set; }
        public DbSet<Harvest> Harvests { get; set; }
    }
}