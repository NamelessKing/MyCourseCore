using Microsoft.AspNetCore.Mvc;
using MyCourseCore.Models.InputModels;
using MyCourseCore.Models.Services.Application;
using MyCourseCore.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCourseCore.Controllers
{
    public class CoursesController : Controller
    {
        private ICachedCourseService CourseService { get; }

        public CoursesController(ICachedCourseService courseService)
        {
            CourseService = courseService;
        }

        public async Task<IActionResult> Index(CourseListInputModel courseListInputModel)
        {
            ViewData["Title"] = "Catalogo dei corsi";
            List<CourseViewModel> courses = await CourseService.GetCoursesAsync(courseListInputModel);
            return View(courses);
        }

        public async Task<IActionResult> Detail(int id)
        {
            CourseDetailViewModel viewModel = await CourseService.GetCourseAsync(id);
            ViewData["Title"] = viewModel.Title;
            return View(viewModel);
        }
    }


}
