using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers
{
    public class CropController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
