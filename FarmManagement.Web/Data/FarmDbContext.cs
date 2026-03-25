using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Data
{
    public class FarmDbContext : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
