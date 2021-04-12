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

        public IActionResult Index()
        {
            ViewData["Title"] = "Catalogo dei corsi";
           List <CourseViewModel> courses = CourseService.GetCourses();
            return View(courses);
        }

        public IActionResult Detail(int id)
        {
            CourseDetailViewModel viewModel = CourseService.GetCourse(id);
            ViewData["Title"] = viewModel.Title;
            return View(viewModel);
        }
    }
}
