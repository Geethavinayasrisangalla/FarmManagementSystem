using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Services
{
    public class ResourceService : IResourceService
    {
    private readonly FarmDbContext _db;
    public ResourceService(FarmDbContext db) => _db = db;

    public async Task<IEnumerable<Resource>> GetAllAsync() =>
        await _db.Resources.OrderBy(r => r.Name).ToListAsync();

    public async Task<Resource?> GetByIdAsync(int id) =>
        await _db.Resources.Include(r => r.ResourceUsages).ThenInclude(ru => ru.Field)
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
        var r = await _db.Resources.FindAsync(vm.ResourceId)
                ?? throw new KeyNotFoundException("Resource not found.");
        r.Name = vm.Name;
        r.Type = vm.Type;
        r.Quantity = vm.Quantity;
        r.Unit = vm.Unit;
        r.LastUpdated = DateTime.Now;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var r = await _db.Resources.FindAsync(id);
        if (r != null) { _db.Resources.Remove(r); await _db.SaveChangesAsync(); }
    }

    public async Task AllocateAsync(int resourceId, int fieldId, decimal qty, string? notes)
    {
        var r = await _db.Resources.FindAsync(resourceId)
                ?? throw new KeyNotFoundException("Resource not found.");
        if (r.Quantity < qty)
            throw new InvalidOperationException($"Insufficient stock. Available: {r.Quantity} {r.Unit}.");
        r.Quantity -= qty;
        _db.ResourceUsages.Add(new ResourceUsage
        {
            ResourceId = resourceId,
            FieldId = fieldId,
            QuantityUsed = qty,
            Notes = notes,
            UsedDate = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Resource>> GetLowStockAsync(decimal threshold = 10) =>
        await _db.Resources.Where(r => r.Quantity <= threshold).ToListAsync();
    }
}
