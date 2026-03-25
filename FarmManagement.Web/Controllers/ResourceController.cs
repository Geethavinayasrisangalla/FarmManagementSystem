using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers
{
    public class ResourceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
