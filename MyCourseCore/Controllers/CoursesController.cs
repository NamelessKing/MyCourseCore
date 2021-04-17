using Microsoft.AspNetCore.Mvc;
using MyCourseCore.Models.Services.Application;
using MyCourseCore.Models.ViewModels;
using System.Collections.Generic;

namespace MyCourseCore.Controllers
{
    public class CoursesController : Controller
    {
        private  ICourseService CourseService { get; }

        public CoursesController(ICourseService courseService)
        {
            CourseService = courseService;
        }

        public async System.Threading.Tasks.Task<IActionResult> IndexAsync()
        {
            ViewData["Title"] = "Catalogo dei corsi";
           List <CourseViewModel> courses = await CourseService.GetCoursesAsync();
            return View(courses);
        }

        public async System.Threading.Tasks.Task<IActionResult> DetailAsync(int id)
        {
            CourseDetailViewModel viewModel = await CourseService.GetCourseAsync(id);
            ViewData["Title"] = viewModel.Title;
            return View(viewModel);
        }
    }
}
