using System;
using Microsoft.AspNetCore.Mvc;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Services;

namespace FarmManagement.Web.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET: /Schedule/Index
        // Shows all planting schedules
        public IActionResult Index()
        {
            var schedules = _scheduleService.GetAllSchedules();
            return View(schedules);
        }

        // GET: /Schedule/CreatePlantingSchedule
        public IActionResult CreatePlantingSchedule()
        {
            return View();
        }

        // POST: /Schedule/CreatePlantingSchedule
        [HttpPost]
        public IActionResult CreatePlantingSchedule(PlantingSchedule schedule)
        {
            if (!ModelState.IsValid)
                return View(schedule);

            bool success = _scheduleService.CreatePlantingSchedule(schedule);

            if (!success)
            {
                ModelState.AddModelError("",
                    "This field is currently Active. " +
                    "Please record a Harvest before planting again.");
                return View(schedule);
            }

            return RedirectToAction("Index");
        }

        // POST: /Schedule/ActivateSchedule/5
        [HttpPost]
        public IActionResult ActivateSchedule(int id)
        {
            bool success = _scheduleService.ActivateSchedule(id);

            if (!success)
                return BadRequest("Schedule not found or is not in Planned status.");

            return RedirectToAction("Index");
        }

        // GET: /Schedule/RecordHarvest
        public IActionResult RecordHarvest()
        {
            return View();
        }

        // POST: /Schedule/RecordHarvest
        [HttpPost]
        public IActionResult RecordHarvest(Harvest harvest)
        {
            if (!ModelState.IsValid)
                return View(harvest);

            bool success = _scheduleService.RecordHarvest(harvest);

            if (!success)
            {
                ModelState.AddModelError("",
                    "Cannot record harvest. Field must be in Active status.");
                return View(harvest);
            }

            return RedirectToAction("Index");
        }

        // GET: /Schedule/GetScheduleDetails/5
        public IActionResult GetScheduleDetails(int id)
        {
            var schedule = _scheduleService.GetScheduleById(id);

            if (schedule == null)
                return NotFound();

            return View(schedule);
        }
    }
}
