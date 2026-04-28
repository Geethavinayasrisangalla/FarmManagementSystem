using FarmManagement.Web.Data;
using FarmManagement.Web.Events;
using FarmManagement.Web.Events.Handlers;
using FarmManagement.Web.Factories;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.Validations;
using FarmManagement.Web.Services;
using FarmManagement.Web.Services.Strategies;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<FarmDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CropValidator>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath       = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan  = TimeSpan.FromHours(8);
    });


builder.Services.AddMemoryCache();


builder.Services.AddScoped<IFieldService,          FieldService>();
builder.Services.AddScoped<IPestService,           PestService>();
builder.Services.AddScoped<IScheduleService,       ScheduleService>();
builder.Services.AddScoped<IReportService,         ReportService>();
builder.Services.AddScoped<IAccountService,        AccountService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IActivityService,       ActivityService>();



builder.Services.AddScoped<CropService>();
builder.Services.AddScoped<ICropService>(provider =>
    new CachedCropService(
        provider.GetRequiredService<CropService>(),
        provider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()));


builder.Services.AddScoped<IAllocationStrategy, StandardAllocationStrategy>();
builder.Services.AddScoped<IResourceService,    ResourceService>();


builder.Services.AddScoped<ICropFactory,      CropFactory>();
builder.Services.AddScoped<IFieldFactory,     FieldFactory>();
builder.Services.AddScoped<IResourceFactory,  ResourceFactory>();


builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();


builder.Services.AddScoped<IEventHandler<CropCreatedEvent>,  CropCreatedHandler>();
builder.Services.AddScoped<IEventHandler<CropUpdatedEvent>,  CropUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<CropDeletedEvent>,  CropDeletedHandler>();


builder.Services.AddScoped<IEventHandler<FieldCreatedEvent>, FieldCreatedHandler>();
builder.Services.AddScoped<IEventHandler<FieldUpdatedEvent>, FieldUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<FieldDeletedEvent>, FieldDeletedHandler>();


builder.Services.AddScoped<IEventHandler<ResourceCreatedEvent>,   ResourceCreatedHandler>();
builder.Services.AddScoped<IEventHandler<ResourceUpdatedEvent>,   ResourceUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<ResourceAllocatedEvent>, ResourceAllocatedHandler>();
builder.Services.AddScoped<IEventHandler<ResourceDeletedEvent>,   ResourceDeletedHandler>();


builder.Services.AddScoped<IEventHandler<PestReportedEvent>,      PestReportedHandler>();
builder.Services.AddScoped<IEventHandler<PestStatusUpdatedEvent>, PestStatusUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<PestDeletedEvent>,       PestDeletedHandler>();


builder.Services.AddScoped<IEventHandler<ScheduleCreatedEvent>,  ScheduleCreatedHandler>();
builder.Services.AddScoped<IEventHandler<HarvestRecordedEvent>,  HarvestRecordedHandler>();
builder.Services.AddScoped<IEventHandler<ScheduleDeletedEvent>,  ScheduleDeletedHandler>();


builder.Services.AddScoped<IFarmFacade, FarmFacade>();


builder.Services.AddSingleton(FarmCacheService.Instance);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<FarmDbContext>();
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying database migrations.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
