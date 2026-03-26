using Microsoft.EntityFrameworkCore;
using FarmManagement.Web.Data;
using FarmManagement.Web.Services; // IMPORTANT: This allows ICropService to be found

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container (Standard MVC setup)
builder.Services.AddControllersWithViews();

// 2. Database Connection (Found in appsettings.json)
builder.Services.AddDbContext<FarmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Dependency Injection (Connects Interface to Implementation)
// This fixes the "ICropService could not be found" error
builder.Services.AddScoped<ICropService, CropService>();
builder.Services.AddScoped<IResourceService, ResourceService>();

var app = builder.Build();

// 4. Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 5. Default Route (Points to Home/Index on startup)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();