using System.Linq;
using System.Threading.Tasks;
using FarmManagement.Web.Data;
using FarmManagement.Web.Models;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarmManagement.Web.Controllers
{
    public class PestController : Controller
    {
        private readonly IPestService _pestService;
        private readonly PestDbContext _context;

        public PestController(IPestService pestService, PestDbContext context)
        {
            _pestService = pestService;
            _context = context; 
        }

        // GET: /Pest/Index (View History)
        public async Task<IActionResult> Index()
        {
            var history = await _pestService.GetPestHistoryAsync();
            return View(history);
        }

        // GET: /Pest/RecordIncident
        public IActionResult RecordIncident()
        {
            // Taking data from Member 1 to populate dropdowns
            ViewBag.Fields = new SelectList(_context.Fields, "FieldId", "FieldName");
            ViewBag.Crops = new SelectList(_context.Crops, "CropId", "CropName");
            return View();
        }

        // POST: /Pest/RecordIncident
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordIncident(PestIncident incident)
        {
            if (ModelState.IsValid)
            {
                await _pestService.RecordPestIncidentAsync(incident);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Fields = new SelectList(_context.Fields, "FieldId", "FieldName");
            ViewBag.Crops = new SelectList(_context.Crops, "CropId", "CropName");
            return View(incident);
        }

        // GET: /Pest/LogTreatment/{incidentId}
        public IActionResult LogTreatment(int incidentId)
        {
            ViewBag.IncidentId = incidentId;
            ViewBag.Resources = new SelectList(_context.Resources.Where(r => r.Type == "PESTICIDE"), "ResourceId", "Name");
            return View();
        }

        // POST: /Pest/LogTreatment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogTreatment(Treatment treatment)
        {
            if (ModelState.IsValid)
            {
                await _pestService.LogTreatmentAsync(treatment);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IncidentId = treatment.IncidentId;
            ViewBag.Resources = new SelectList(_context.Resources.Where(r => r.Type == "PESTICIDE"), "ResourceId", "Name");
            return View(treatment);
        }
    }
}
