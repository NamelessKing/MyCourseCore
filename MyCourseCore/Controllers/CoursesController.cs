using Microsoft.AspNetCore.Mvc;

namespace MyCourseCore.Controllers
{
    public class CoursesController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(string id)
        {
            return View();
        }
    }
}
