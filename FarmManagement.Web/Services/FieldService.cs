using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

// Template Method Pattern — extends BaseEntityService which defines the Create skeleton.
// FieldService implements BuildEntity and adds a location-required validation hook.
public class FieldService : BaseEntityService<Field, FieldViewModel>, IFieldService
{
    public FieldService(FarmDbContext db) : base(db) { }

    // ── Template Method hooks ────────────────────────────────────────────────

    protected override void ValidateViewModel(FieldViewModel vm)
    {
        if (vm.AreaHectares <= 0)
            throw new ArgumentException("Field area must be greater than zero.");
    }

    protected override Field BuildEntity(FieldViewModel vm) => new Field
    {
        FieldName    = vm.FieldName,
        AreaHectares = vm.AreaHectares,
        SoilType     = vm.SoilType,
        Location     = vm.Location,
        CreatedAt    = DateTime.Now
    };

    // ── IFieldService implementation ─────────────────────────────────────────

    public async Task<IEnumerable<Field>> GetAllAsync() =>
        await _db.Fields.Include(f => f.Crops)
                        .OrderByDescending(f => f.FieldId)
                        .ToListAsync();

    public async Task<Field?> GetByIdAsync(int id) =>
        await _db.Fields.Include(f => f.Crops)
                        .Include(f => f.PlantingSchedules)
                        .FirstOrDefaultAsync(f => f.FieldId == id);

    // Delegates to the base template method — Validate → BuildEntity → Save → AfterCreate
    public async Task CreateAsync(FieldViewModel vm) => await TemplateCreateAsync(vm);

    public async Task UpdateAsync(FieldViewModel vm)
    {
        var field = await _db.Fields.FindAsync(vm.FieldId)
                    ?? throw new KeyNotFoundException("Field not found.");
        field.FieldName    = vm.FieldName;
        field.AreaHectares = vm.AreaHectares;
        field.SoilType     = vm.SoilType;
        field.Location     = vm.Location;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var field = await _db.Fields.FindAsync(id);
        if (field != null) { _db.Fields.Remove(field); await _db.SaveChangesAsync(); }
    }
}
