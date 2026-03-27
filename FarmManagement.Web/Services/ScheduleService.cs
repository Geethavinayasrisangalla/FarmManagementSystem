using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarmManagement.Web.Services
{
    public class ScheduleService : IScheduleService
    {
    private readonly FarmDbContext _db;
    public ScheduleService(FarmDbContext db) => _db = db;

    public async Task<IEnumerable<PlantingSchedule>> GetAllAsync() =>
        await _db.PlantingSchedules.Include(ps => ps.Crop)
                                   .OrderBy(ps => ps.ScheduledDate)
                                   .ToListAsync();

    public async Task<PlantingSchedule?> GetByIdAsync(int id) =>
        await _db.PlantingSchedules.Include(ps => ps.Crop)
                                   .Include(ps => ps.Harvest)
                                   .FirstOrDefaultAsync(ps => ps.PlantingScheduleId == id);

    public async Task CreateAsync(ScheduleViewModel vm)
    {
        _db.PlantingSchedules.Add(new PlantingSchedule
        {
            CropId = vm.CropId,
            ScheduledDate = vm.ScheduledDate,
            ExpectedYieldKg = vm.ExpectedYieldKg,
            Notes = vm.Notes,
            Status = "Scheduled"
        });
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(ScheduleViewModel vm)
    {
        var ps = await _db.PlantingSchedules.FindAsync(vm.PlantingScheduleId)
                 ?? throw new KeyNotFoundException("Schedule not found.");
        ps.CropId = vm.CropId;
        ps.ScheduledDate = vm.ScheduledDate;
        ps.ExpectedYieldKg = vm.ExpectedYieldKg;
        ps.Notes = vm.Notes;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var ps = await _db.PlantingSchedules.FindAsync(id);
        if (ps != null) { _db.PlantingSchedules.Remove(ps); await _db.SaveChangesAsync(); }
    }

    public async Task RecordHarvestAsync(int scheduleId, decimal actualYield, string? notes)
    {
        var ps = await _db.PlantingSchedules.FindAsync(scheduleId)
                 ?? throw new KeyNotFoundException("Schedule not found.");
        ps.Status = "Completed";
        _db.Harvests.Add(new Harvest
        {
            PlantingScheduleId = scheduleId,
            ActualYieldKg = actualYield,
            QualityNotes = notes,
            HarvestedDate = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<PlantingSchedule>> GetUpcomingAsync(int days = 30) =>
        await _db.PlantingSchedules.Include(ps => ps.Crop)
                                   .Where(ps => ps.ScheduledDate <= DateTime.Today.AddDays(days)
                                             && ps.Status == "Scheduled")
                                   .OrderBy(ps => ps.ScheduledDate)
                                   .ToListAsync();

    public async Task<ScheduleViewModel> PrepareViewModelAsync(ScheduleViewModel? vm = null)
    {
        vm ??= new ScheduleViewModel();
        var crops = await _db.Crops.Where(c => c.Status == "Growing")
                                   .OrderBy(c => c.CropName)
                                   .ToListAsync();
        vm.Crops = crops.Select(c => new SelectListItem
        {
            Value = c.CropId.ToString(),
            Text = c.CropName,
            Selected = c.CropId == vm.CropId
        }).ToList();
        return vm;
    }
    }
}
