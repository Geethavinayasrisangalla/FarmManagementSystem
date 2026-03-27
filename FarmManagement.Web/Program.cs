// ============================================================
// Program.cs — FarmManagement.Web
// ============================================================
using FarmManagement.Web.Data;
using FarmManagement.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────
// Step 1: Add MVC
// ─────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ─────────────────────────────────────────
// Step 2: Connect Database (FarmDbContext)
// ─────────────────────────────────────────
builder.Services.AddDbContext<FarmDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// ─────────────────────────────────────────
// Step 3: Register All 5 Member Services
// ─────────────────────────────────────────

// Member 1 — Crop & Field Management
builder.Services.AddScoped<CropService>();
builder.Services.AddScoped<FieldService>();

// Member 2 — Resource & Inventory
builder.Services.AddScoped<ResourceService>();

// Member 3 — Planting & Harvest Scheduling
builder.Services.AddScoped<ScheduleService>();

// Member 4 — Pest & Treatment
builder.Services.AddScoped<PestService>();

// Member 5 — Yield Analytics & Reporting
builder.Services.AddScoped<ReportService>();

// ─────────────────────────────────────────
// Step 4: Build the App
// ─────────────────────────────────────────
var app = builder.Build();

// ─────────────────────────────────────────
// Step 5: Middleware Pipeline
// ─────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ─────────────────────────────────────────
// Step 6: Default Route
// ─────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();