using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers
{
    public class ScheduleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
