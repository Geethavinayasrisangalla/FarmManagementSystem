// ── IResourceService.cs ──────────────────────────────────────
using FarmManagement.Models.ViewModels;

public interface IResourceService
{
    Task<IEnumerable<Resource>> GetAllAsync();
    Task<Resource?> GetByIdAsync(int id);
    Task CreateAsync(InventoryViewModel vm);
    Task UpdateAsync(InventoryViewModel vm);
    Task DeleteAsync(int id);
    Task AllocateAsync(int resourceId, int fieldId, decimal qty, string? notes);
    Task<IEnumerable<Resource>> GetLowStockAsync(decimal threshold = 10);
}

