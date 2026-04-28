using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Services.Strategies;


public interface IAllocationStrategy
{
    Task AllocateAsync(Resource resource, decimal quantity);
}
