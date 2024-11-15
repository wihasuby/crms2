using Microsoft.AspNetCore.Mvc;

namespace crms2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // This follows the convention and looks for Views/Home/Index.cshtml
        }

    }
}
