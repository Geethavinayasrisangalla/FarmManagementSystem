using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class ResourceService : IResourceService
{
    private readonly FarmDbContext _db;
    public ResourceService(FarmDbContext db) => _db = db;

    public async Task<IEnumerable<Resource>> GetAllAsync() =>
        await _db.Resources.OrderBy(r => r.Name).ToListAsync();

    public async Task<Resource?> GetByIdAsync(int id) =>
        await _db.Resources
                 .Include(r => r.ResourceUsages)
                     .ThenInclude(ru => ru.PlantingSchedule) // fixed: was ru.Field
                 .FirstOrDefaultAsync(r => r.ResourceId == id);

    public async Task CreateAsync(InventoryViewModel vm)
    {
        _db.Resources.Add(new Resource
        {
            Name = vm.Name,
            Type = vm.Type,
            Quantity = vm.Quantity,
            Unit = vm.Unit,
            LastUpdated = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(InventoryViewModel vm)
    {
        var resource = await _db.Resources.FindAsync(vm.ResourceId)
                       ?? throw new KeyNotFoundException("Resource not found.");
        resource.Name = vm.Name;
        resource.Type = vm.Type;
        resource.Quantity = vm.Quantity;
        resource.Unit = vm.Unit;
        resource.LastUpdated = DateTime.Now;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var resource = await _db.Resources.FindAsync(id);
        if (resource != null) { _db.Resources.Remove(resource); await _db.SaveChangesAsync(); }
    }

    public async Task AllocateAsync(int resourceId, int scheduleId, decimal qty, string? notes)
    {                                           // fixed: fieldId → scheduleId
        var resource = await _db.Resources.FindAsync(resourceId)
                       ?? throw new KeyNotFoundException("Resource not found.");

        if (resource.Quantity < qty)
            throw new InvalidOperationException(
                $"Insufficient stock. Available: {resource.Quantity} {resource.Unit}.");

        resource.Quantity -= qty;
        resource.LastUpdated = DateTime.Now;

        _db.ResourceUsages.Add(new ResourceUsage
        {
            ResourceId = resourceId,
            ScheduleId = scheduleId,          // fixed: was FieldId
            QuantityUsed = qty,
            Notes = notes,
            UsedDate = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Resource>> GetLowStockAsync(decimal threshold = 10) =>
        await _db.Resources.Where(r => r.Quantity <= threshold).ToListAsync();
}