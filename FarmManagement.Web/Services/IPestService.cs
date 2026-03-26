using FarmManagement.Web.Models;
using FarmManagement.Web.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmManagement.Web.Services
{
    public interface IPestService
    {
        Task<int> RecordPestIncidentAsync(PestIncident incident);
        Task<int> LogTreatmentAsync(Treatment treatment);
        Task<IEnumerable<PestIncident>> GetPestHistoryAsync(); 
    }
}
