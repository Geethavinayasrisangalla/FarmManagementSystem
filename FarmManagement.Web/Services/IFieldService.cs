using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Services
{
    public interface IFieldService
    {
        Task<IEnumerable<Field>> GetAllFieldsAsync();
        Task<Field?> GetFieldByIdAsync(int id);
        Task AddFieldAsync(Field field);
        Task UpdateFieldAsync(Field field);
        Task DeleteFieldAsync(int id);
    }
}