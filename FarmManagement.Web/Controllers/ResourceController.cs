using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers
{
    public class ResourceController : Controller
    {
        // Simple in-memory store for example purposes
        private static readonly List<Resource> _store = new()
        {
            new Resource { Id = 1, Name = "Tractor", Quantity = 2 },
            new Resource { Id = 2, Name = "Irrigation Pump", Quantity = 1 }
        };

        public IActionResult Index()
        {
            return View(_store);
        }

        public IActionResult Details(int id)
        {
            var resource = _store.FirstOrDefault(r => r.Id == id);
            if (resource == null) return NotFound();
            return View(resource);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Resource resource)
        {
            if (!ModelState.IsValid) return View(resource);

            resource.Id = _store.Any() ? _store.Max(r => r.Id) + 1 : 1;
            _store.Add(resource);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var resource = _store.FirstOrDefault(r => r.Id == id);
            if (resource == null) return NotFound();
            return View(resource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Resource updated)
        {
            if (!ModelState.IsValid) return View(updated);

            var resource = _store.FirstOrDefault(r => r.Id == id);
            if (resource == null) return NotFound();

            resource.Name = updated.Name;
            resource.Quantity = updated.Quantity;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var resource = _store.FirstOrDefault(r => r.Id == id);
            if (resource != null) _store.Remove(resource);
            return RedirectToAction(nameof(Index));
        }
    }

    public class Resource
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}