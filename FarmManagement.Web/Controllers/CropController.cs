using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmManagement.Web.Data;             // Connects to your Database folder
using FarmManagement.Web.Services;         // Connects to your Services folder
using FarmManagement.Web.Models.Entities;  // Connects to Crop/Field entities
using FarmManagement.Web.Models.ViewModels;// Connects to your CropViewModel

using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers
{
    public class CropController : Controller
    {
        private readonly ICropService _cropService;
        public CropController(ICropService cropService) => _cropService = cropService;

        public async Task<IActionResult> Index()
        {
            var crops = await _cropService.GetAllCropsAsync();
            return View(crops);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CropViewModel model)
        {
            if (ModelState.IsValid)
            {
                var crop = new Crop
                {
                    CommonName = model.CommonName,
                    ScientificName = model.ScientificName,
                    Category = model.Category,
                    RecommendedSeason = model.RecommendedSeason
                };
                await _cropService.AddNewCropAsync(crop);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}