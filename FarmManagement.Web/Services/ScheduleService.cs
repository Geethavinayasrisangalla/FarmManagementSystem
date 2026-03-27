using System;
using System.Collections.Generic;
using System.Linq;
using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Services
{
    public class ScheduleService
    {
        private readonly FarmDbContext _context;

        public ScheduleService(FarmDbContext context)
        {
            _context = context;
        }

        // --- PLANTING SCHEDULE METHODS ---

        // Create a new planting schedule (Status = "Planned")
        public bool CreatePlantingSchedule(PlantingSchedule schedule)
        {
            // Check if the field is already Active (planted but not harvested)
            bool fieldAlreadyActive = _context.PlantingSchedules
                .Any(s => s.FieldId == schedule.FieldId && s.Status == "Active");

            if (fieldAlreadyActive)
            {
                // Cannot plant again until current harvest is recorded
                return false;
            }

            schedule.Status = "Planned";
            _context.PlantingSchedules.Add(schedule);
            _context.SaveChanges();
            return true;
        }

        // Get all planting schedules
        public List<PlantingSchedule> GetAllSchedules()
        {
            return _context.PlantingSchedules.ToList();
        }

        // Get a single schedule by ID
        public PlantingSchedule GetScheduleById(int scheduleId)
        {
            return _context.PlantingSchedules
                .FirstOrDefault(s => s.ScheduleId == scheduleId);
        }

        // Activate a schedule (Planned → Active)
        public bool ActivateSchedule(int scheduleId)
        {
            var schedule = _context.PlantingSchedules
                .FirstOrDefault(s => s.ScheduleId == scheduleId);

            if (schedule == null || schedule.Status != "Planned")
                return false;

            schedule.Status = "Active";
            _context.SaveChanges();
            return true;
        }

        // --- HARVEST METHODS ---

        // Record a harvest and mark schedule as Completed (Active → Completed)
        public bool RecordHarvest(Harvest harvest)
        {
            // Find the related schedule
            var schedule = _context.PlantingSchedules
                .FirstOrDefault(s => s.ScheduleId == harvest.ScheduleId);

            if (schedule == null || schedule.Status != "Active")
            {
                // Can only harvest an Active field
                return false;
            }

            // Save the harvest record
            _context.Harvests.Add(harvest);

            // Mark the schedule as Completed
            schedule.Status = "Completed";

            _context.SaveChanges();
            return true;
        }

        // Get all harvests
        public List<Harvest> GetAllHarvests()
        {
            return _context.Harvests.ToList();
        }

        // Get harvest by field (used by Member 5 for yield calculation)
        public List<Harvest> GetHarvestsByField(int fieldId)
        {
            return _context.Harvests
                .Where(h => h.FieldId == fieldId)
                .ToList();
        }
    }
}
