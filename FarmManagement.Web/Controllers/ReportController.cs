using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
