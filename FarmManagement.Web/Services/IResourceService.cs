using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Services
{
    public interface IResourceService
    {
        Task<List<Resource>> GetAllAsync();
        Task<Resource?> GetByIdAsync(int id);
        Task<Resource> CreateAsync(Resource resource);
        Task<Resource?> UpdateAsync(Resource resource);
        Task<bool> DeleteAsync(int id);

        // Usage operation: creates usage and deducts inventory atomically
        Task<(bool Success, string? ErrorMessage)> CreateUsageAsync(ResourceUsage usage);
    }
}