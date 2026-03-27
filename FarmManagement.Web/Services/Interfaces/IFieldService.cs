// ── IFieldService.cs ─────────────────────────────────────────
using FarmManagement.Models.ViewModels;
using FarmManagement.Web.Models.Entities;

public interface IFieldService
{
    Task<IEnumerable<Field>> GetAllAsync();
    Task<Field?> GetByIdAsync(int id);
    Task CreateAsync(FieldViewModel vm);
    Task UpdateAsync(FieldViewModel vm);
    Task DeleteAsync(int id);
}