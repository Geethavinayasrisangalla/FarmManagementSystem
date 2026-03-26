using Microsoft.EntityFrameworkCore;
using FarmManagement.Web.Data;
using FarmManagement.Web.Models;

namespace FarmManagement.Web.Services
{
    public class ResourceService : IResourceService
    {
        private readonly ApplicationDbContext _db;
        public ResourceService(ApplicationDbContext db) => _db = db;

        public async Task<List<Resource>> GetAllAsync()
            => await _db.Resources.AsNoTracking().ToListAsync();

        public async Task<Resource?> GetByIdAsync(int id)
            => await _db.Resources.Include(r => r.Usages).FirstOrDefaultAsync(r => r.ResourceId == id);

        public async Task<Resource> CreateAsync(Resource resource)
        {
            _db.Resources.Add(resource);
            await _db.SaveChangesAsync();
            return resource;
        }

        public async Task<Resource?> UpdateAsync(Resource resource)
        {
            var existing = await _db.Resources.FindAsync(resource.ResourceId);
            if (existing == null) return null;
            existing.Name = resource.Name;
            existing.Type = resource.Type;
            existing.Unit = resource.Unit;
            existing.QuantityAvailable = resource.QuantityAvailable;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _db.Resources.FindAsync(id);
            if (e == null) return false;
            _db.Resources.Remove(e);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateUsageAsync(ResourceUsage usage)
        {
            // Validate resource exists
            var resource = await _db.Resources.FirstOrDefaultAsync(r => r.ResourceId == usage.ResourceId);
            if (resource == null) return (false, "Resource not found");

            if (usage.QuantityUsed <= 0) return (false, "Quantity used must be > 0");

            if (usage.QuantityUsed > resource.QuantityAvailable)
            {
                return (false, $"Not enough inventory. Available: {resource.QuantityAvailable}");
            }

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // deduct
                resource.QuantityAvailable -= (int)Math.Ceiling(usage.QuantityUsed); // coerce to int if you're storing ints
                _db.ResourceUsages.Add(usage);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (false, ex.Message);
            }
        }
    }
}