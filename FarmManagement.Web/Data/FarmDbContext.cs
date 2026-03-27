using Microsoft.EntityFrameworkCore;
using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Data
{
    public class FarmDbContext : DbContext
    {
        public FarmDbContext(DbContextOptions<FarmDbContext> options) : base(options) { }

        // Member 1: Core Tables
        public DbSet<Crop> Crops { get; set; }
        public DbSet<Field> Fields { get; set; }

        // Member 3: Planting Tables
        public DbSet<PlantingSchedule> PlantingSchedules { get; set; }
        public DbSet<Harvest> Harvests { get; set; }

        // Member 4: Pest & Health Tables (ADD THESE TO FIX THE ERRORS)
        public DbSet<PestIncident> PestIncidents { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
    }
}