using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services
{
    public class CropService : ICropService
    {
        private readonly FarmDbContext _context;

        // Dependency Injection: Bringing in the Database Context
        public CropService(FarmDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Crop>> GetAllCropsAsync()
        {
            return await _context.Crops.ToListAsync();
        }

        public async Task<Crop?> GetCropByIdAsync(int id)
        {
            return await _context.Crops.FirstOrDefaultAsync(c => c.CropId == id);
        }

        public async Task AddNewCropAsync(Crop crop)
        {
            _context.Crops.Add(crop);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCropAsync(Crop crop)
        {
            _context.Crops.Update(crop);
            await _context.SaveChangesAsync();
        }
    }
}