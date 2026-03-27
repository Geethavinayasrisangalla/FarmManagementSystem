using Microsoft.EntityFrameworkCore;
using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Services
{
    public class ResourceService : IResourceService
    {
        private readonly FarmDbContext _db;

        public ResourceService(FarmDbContext db) => _db = db;

        // ─── Get All Resources ───────────────────────────────────────────────
        public async Task<List<Resource>> GetAllAsync()
            => await _db.Resources.AsNoTracking().ToListAsync();

        // ─── Get Single Resource with its Usages ────────────────────────────
        public async Task<Resource?> GetByIdAsync(int id)
            => await _db.Resources
                .Include(r => r.Usages!)
                    .ThenInclude(u => u.Field)
                .FirstOrDefaultAsync(r => r.ResourceId == id);

        // ─── Create Resource (add stock to warehouse) ───────────────────────
        public async Task<Resource> CreateAsync(Resource resource)
        {
            _db.Resources.Add(resource);
            await _db.SaveChangesAsync();
            return resource;
        }

        // ─── Update Resource (e.g. restock / rename) ────────────────────────
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

        // ─── Delete Resource ────────────────────────────────────────────────
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Resources.FindAsync(id);
            if (entity == null) return false;

            _db.Resources.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        // ─── Get All Usages for a Resource ──────────────────────────────────
        public async Task<List<ResourceUsage>> GetUsagesByResourceAsync(int resourceId)
            => await _db.ResourceUsage
                .AsNoTracking()
                .Include(u => u.Field)
                .Where(u => u.ResourceId == resourceId)
                .OrderByDescending(u => u.DateApplied)
                .ToListAsync();

        // ─── KEY LOGIC: Log usage & deduct inventory atomically ─────────────
        //     This is the "crucial logic" from the work division document:
        //     quantityUsed cannot exceed quantityAvailable.
        public async Task<(bool Success, string? ErrorMessage)> CreateUsageAsync(ResourceUsage usage)
        {
            // 1. Load the resource (with tracking so EF picks up changes)
            var resource = await _db.Resources.FirstOrDefaultAsync(r => r.ResourceId == usage.ResourceId);
            if (resource == null)
                return (false, "Resource not found.");

            // 2. Validate positive quantity
            if (usage.QuantityUsed <= 0)
                return (false, "Quantity used must be greater than zero.");

            // 3. CRUCIAL CHECK: prevent over-use
            if (usage.QuantityUsed > resource.QuantityAvailable)
                return (false, $"Insufficient stock. Available: {resource.QuantityAvailable} {resource.Unit}.");

            // 4. Atomic transaction: deduct + log
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                resource.QuantityAvailable -= usage.QuantityUsed;
                _db.ResourceUsage.Add(usage);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (false, $"Transaction failed: {ex.Message}");
            }
        }
    }
}