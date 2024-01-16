using Microsoft.AspNetCore.Mvc;

namespace MovieStoreMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
