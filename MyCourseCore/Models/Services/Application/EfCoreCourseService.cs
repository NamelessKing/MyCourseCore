using Microsoft.EntityFrameworkCore;
using MyCourseCore.Models.Entities;
using MyCourseCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Services.Application
{
    public class EfCoreCourseService : ICourseService
    {
        public EfCoreCourseService(MyCourseDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private MyCourseDbContext DbContext { get; }

        public Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            List<CourseViewModel> courses = await DbContext.Courses.Select(course => 
            new CourseViewModel() { 
                Id = course.Id,
                Author = course.Author,
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice,
                ImagePath = course.ImagePath,
                Rating = course.Rating,
                Title = course.Title
            }).ToListAsync();

            return courses;
        }
    }
}
