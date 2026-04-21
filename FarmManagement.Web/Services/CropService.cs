using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class CropService : ICropService
{
    private readonly FarmDbContext _db;
    public CropService(FarmDbContext db) => _db = db;

    public async Task<IEnumerable<Crop>> GetAllAsync() =>
        await _db.Crops.AsNoTracking()
                       .Include(c => c.Field)
                       .OrderByDescending(c => c.CropId)
                       .ToListAsync();

    public async Task<Crop?> GetByIdAsync(int id) =>
        await _db.Crops.AsNoTracking()
                       .Include(c => c.Field)
                       .Include(c => c.PestIncidents)
                       .Include(c => c.PlantingSchedules)
                       .FirstOrDefaultAsync(c => c.CropId == id);

    public async Task CreateAsync(CropViewModel vm)
    {
        _db.Crops.Add(new Crop
        {
            CropName = vm.CropName,
            CropType = vm.CropType,
            Season = vm.Season,
            PlantingDate = vm.PlantingDate,
            ExpectedHarvestDate = vm.ExpectedHarvestDate,
            FieldId = vm.FieldId,
            Status = "Growing"
        });
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(CropViewModel vm)
    {
        var crop = await _db.Crops.FindAsync(vm.CropId)
                   ?? throw new KeyNotFoundException("Crop not found.");
        crop.CropName = vm.CropName;
        crop.CropType = vm.CropType;
        crop.Season = vm.Season;
        crop.PlantingDate = vm.PlantingDate;
        crop.ExpectedHarvestDate = vm.ExpectedHarvestDate;
        crop.FieldId = vm.FieldId;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var crop = await _db.Crops.FindAsync(id);
        if (crop != null) { _db.Crops.Remove(crop); await _db.SaveChangesAsync(); }
    }

    public async Task<CropViewModel> PrepareViewModelAsync(CropViewModel? vm = null)
    {
        vm ??= new CropViewModel();
        var fields = await _db.Fields.OrderBy(f => f.FieldName).ToListAsync();
        vm.Fields = fields.Select(f => new SelectListItem
        {
            Value = f.FieldId.ToString(),
            Text = $"{f.FieldName} ({f.Location})",
            Selected = f.FieldId == vm.FieldId
        }).ToList();
        return vm;
    }
}