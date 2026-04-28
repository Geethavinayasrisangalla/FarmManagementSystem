using FarmManagement.Web.Events;
using FarmManagement.Web.Models.Interfaces;

namespace FarmManagement.Web.Services;



public class FarmFacade : IFarmFacade
{
    private readonly IResourceService _resourceService;
    private readonly IScheduleService _scheduleService;
    private readonly ICropService     _cropService;
    private readonly IFieldService    _fieldService;
    private readonly IPestService     _pestService;
    private readonly IEventDispatcher _dispatcher;

    public FarmFacade(
        IResourceService resourceService,
        IScheduleService scheduleService,
        ICropService     cropService,
        IFieldService    fieldService,
        IPestService     pestService,
        IEventDispatcher dispatcher)
    {
        _resourceService = resourceService;
        _scheduleService = scheduleService;
        _cropService     = cropService;
        _fieldService    = fieldService;
        _pestService     = pestService;
        _dispatcher      = dispatcher;
    }


    public async Task AllocateResourceAsync(int resourceId, int scheduleId, decimal qty,
        string? notes, int userId, string userName, string role)
    {
        await _resourceService.AllocateAsync(resourceId, scheduleId, qty, notes);
        var resource = await _resourceService.GetByIdAsync(resourceId);
        await _dispatcher.DispatchAsync(new ResourceAllocatedEvent(
            userId, userName, role, resource!.Name, qty, resource.Unit, scheduleId));
    }


    public async Task RecordHarvestAsync(int scheduleId, decimal actualYield,
        string? notes, int userId, string userName, string role)
    {
        await _scheduleService.RecordHarvestAsync(scheduleId, actualYield, notes);
        await _dispatcher.DispatchAsync(new HarvestRecordedEvent(
            userId, userName, role, scheduleId, actualYield));
    }


    public async Task DeleteCropAsync(int cropId, string cropName,
        int userId, string userName, string role)
    {
        await _cropService.DeleteAsync(cropId);
        await _dispatcher.DispatchAsync(new CropDeletedEvent(userId, userName, role, cropName));
    }


    public async Task DeleteFieldAsync(int fieldId, string fieldName,
        int userId, string userName, string role)
    {
        await _fieldService.DeleteAsync(fieldId);
        await _dispatcher.DispatchAsync(new FieldDeletedEvent(userId, userName, role, fieldName));
    }


    public async Task DeleteResourceAsync(int resourceId, string resourceName,
        int userId, string userName, string role)
    {
        await _resourceService.DeleteAsync(resourceId);
        await _dispatcher.DispatchAsync(new ResourceDeletedEvent(userId, userName, role, resourceName));
    }


    public async Task DeletePestAsync(int pestId, string pestName,
        int userId, string userName, string role)
    {
        await _pestService.DeleteAsync(pestId);
        await _dispatcher.DispatchAsync(new PestDeletedEvent(userId, userName, role, pestName));
    }


    public async Task DeleteScheduleAsync(int scheduleId,
        int userId, string userName, string role)
    {
        await _scheduleService.DeleteAsync(scheduleId);
        await _dispatcher.DispatchAsync(new ScheduleDeletedEvent(userId, userName, role, scheduleId));
    }
}
