using Microsoft.AspNetCore.Mvc;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Services;

namespace FarmManagement.Web.Controllers
{
    public class FieldController : Controller
    {
        private readonly IFieldService _fieldService;

        public FieldController(IFieldService fieldService)
        {
            _fieldService = fieldService;
        }

        // GET: Field Mapping (List)
        public async Task<IActionResult> Index()
        {
            var fields = await _fieldService.GetAllFieldsAsync();
            return View(fields);
        }

        // GET: Create Field Form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Save New Field
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Field field)
        {
            if (ModelState.IsValid)
            {
                await _fieldService.AddFieldAsync(field);
                return RedirectToAction(nameof(Index));
            }
            return View(field);
        }
    }
}