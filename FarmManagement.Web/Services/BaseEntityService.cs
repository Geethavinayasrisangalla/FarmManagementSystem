using FarmManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;




public abstract class BaseEntityService<TEntity, TViewModel>
    where TEntity : class
{
    protected readonly FarmDbContext _db;

    protected BaseEntityService(FarmDbContext db) => _db = db;



    protected async Task TemplateCreateAsync(TViewModel vm)
    {
        ValidateViewModel(vm);
        var entity = BuildEntity(vm);
        await _db.Set<TEntity>().AddAsync(entity);
        await _db.SaveChangesAsync();
        await AfterCreateAsync(entity);
    }


    protected virtual void ValidateViewModel(TViewModel vm) { }


    protected abstract TEntity BuildEntity(TViewModel vm);


    protected virtual Task AfterCreateAsync(TEntity entity) => Task.CompletedTask;
}
