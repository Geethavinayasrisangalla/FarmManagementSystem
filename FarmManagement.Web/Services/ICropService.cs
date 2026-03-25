using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Services
{
    public interface ICropService
    {
        // Get all crops from the database
        Task<IEnumerable<Crop>> GetAllCropsAsync();

        // Get one specific crop by its ID
        Task<Crop?> GetCropByIdAsync(int id);

        // Save a new crop to the database
        Task AddNewCropAsync(Crop crop);

        // Optional: Update a crop (Add this later if needed)
        Task UpdateCropAsync(Crop crop);
    }
}