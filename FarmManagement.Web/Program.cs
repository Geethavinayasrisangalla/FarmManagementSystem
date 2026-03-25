using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Interfaces;
using System;

builder.Services.AddDbContext<FarmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICropService, CropService>();