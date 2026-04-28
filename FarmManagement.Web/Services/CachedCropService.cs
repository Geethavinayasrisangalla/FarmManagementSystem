using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace FarmManagement.Web.Services;




public class CachedCropService : ICropService
{
    private readonly ICropService  _inner;
    private readonly IMemoryCache  _cache;
    private const    string        AllKey = "crops_all";

    public CachedCropService(ICropService inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }


    public async Task<IEnumerable<Crop>> GetAllAsync()
    {
        if (_cache.TryGetValue(AllKey, out IEnumerable<Crop>? cached))
            return cached!;

        var crops = await _inner.GetAllAsync();
        _cache.Set(AllKey, crops, TimeSpan.FromMinutes(5));
        return crops;
    }


    public Task<Crop?> GetByIdAsync(int id) => _inner.GetByIdAsync(id);

    public async Task CreateAsync(CropViewModel vm)
    {
        await _inner.CreateAsync(vm);
        _cache.Remove(AllKey);
    }

    public async Task UpdateAsync(CropViewModel vm)
    {
        await _inner.UpdateAsync(vm);
        _cache.Remove(AllKey);
    }

    public async Task DeleteAsync(int id)
    {
        await _inner.DeleteAsync(id);
        _cache.Remove(AllKey);
    }


    public Task<CropViewModel> PrepareViewModelAsync(CropViewModel? vm = null)
        => _inner.PrepareViewModelAsync(vm);
}
