using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services
{
    public class FieldService : IFieldService
    {
        private readonly FarmDbContext _context;

        public FieldService(FarmDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Field>> GetAllFieldsAsync()
        {
            return await _context.Fields.ToListAsync();
        }

        public async Task<Field?> GetFieldByIdAsync(int id)
        {
            return await _context.Fields.FindAsync(id);
        }

        public async Task AddFieldAsync(Field field)
        {
            _context.Fields.Add(field);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFieldAsync(Field field)
        {
            _context.Update(field);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFieldAsync(int id)
        {
            var field = await _context.Fields.FindAsync(id);
            if (field != null)
            {
                _context.Fields.Remove(field);
                await _context.SaveChangesAsync();
            }
        }
    }
}