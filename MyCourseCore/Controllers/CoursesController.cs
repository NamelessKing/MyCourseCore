using Microsoft.AspNetCore.Mvc;

namespace MyCourseCore.Controllers
{
    public class CoursesController : Controller
    {

        public IActionResult Index()
        {
            return Content("index");
        }

        public IActionResult Detail(string id)
        {
            return Content($"detail id : {id}");
        }
    }
}
