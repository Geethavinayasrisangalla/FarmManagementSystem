using FarmManagement.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using FarmManagement.Web.Data;

namespace FarmManagement.Web.Services
{
    public class PestService : IPestService
    {
        private readonly PestDbContext _context;

        public PestService(PestDbContext context)
        {
            _context = context;
        }
        public async Task<int> RecordPestIncidentAsync(PestIncident incident)
        {
            _context.PestIncidents.Add(incident);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> LogTreatmentAsync(Treatment treatment)
        {
            _context.Treatments.Add(treatment);

            // IMP: call treatment.QuantityUsed to update the inventory of the pesticide
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PestIncident>> GetPestHistoryAsync()
        {
            return await _context.PestIncidents
                .Include(pi => pi.Treatments)
                .ToListAsync();

        }
    }
}
