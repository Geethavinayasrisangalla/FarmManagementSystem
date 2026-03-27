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

        // Usage operations
        Task<List<ResourceUsage>> GetUsagesByResourceAsync(int resourceId);
        Task<(bool Success, string? ErrorMessage)> CreateUsageAsync(ResourceUsage usage);
    }
}