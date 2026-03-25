using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using FarmManagement.Web.Services;
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