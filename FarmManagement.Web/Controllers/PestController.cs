using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers
{
    public class PestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
