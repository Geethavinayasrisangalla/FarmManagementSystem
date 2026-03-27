using FarmManagement.Web.Data;
using FarmManagement.Web.Services.Interfaces;
using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Validations;
using FarmManagement.Web.Services;
using FarmManagement.Web.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────
builder.Services.AddDbContext<FarmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── MVC ──────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── FluentValidation (correct way for v11) ───────────────────
// FluentValidation packages are optional. If you have FluentValidation installed, register validators here.
// builder.Services.AddFluentValidationAutoValidation();
// builder.Services.AddFluentValidationClientsideAdapters();
// builder.Services.AddValidatorsFromAssemblyContaining<CropValidator>();

// ── Services (DI) ────────────────────────────────────────────
builder.Services.AddScoped<ICropService, CropService>();
builder.Services.AddScoped<IFieldService, FieldService>();
builder.Services.AddScoped<IPestService, PestService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IReportService, ReportService>();

// ── Session ──────────────────────────────────────────────────
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();