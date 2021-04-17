using MyCourseCore.Models.Services.Infrastructure;
using MyCourseCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Services.Application
{
    public class AdoNetCourseService : ICourseService
    {
        public IDatabaseAccessor DatabaseAccessor { get; }

        public AdoNetCourseService(IDatabaseAccessor databaseAccessor)
        {
            DatabaseAccessor = databaseAccessor;
        }


        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, 
                            CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Id={id};
                            SELECT Id, Title, Description, Duration FROM Lessons WHERE CourseId={id}";

            DataSet dataSet = await DatabaseAccessor.QueryAsync(query);

            //Course
            var courseTable = dataSet.Tables[0];
            if (courseTable.Rows.Count != 1)
            {
                throw new InvalidOperationException($"Did not return exactly 1 row for Course {id}");
            }
            var courseRow = courseTable.Rows[0];
            var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);

            //Course lessons
            var lessonDataTable = dataSet.Tables[1];

            foreach (DataRow lessonRow in lessonDataTable.Rows)
            {
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
                courseDetailViewModel.Lessons.Add(lessonViewModel);
            }
            return courseDetailViewModel;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            FormattableString query = $@"SELECT Id, Title, ImagePath, Author, Rating,
                FullPrice_Amount, FullPrice_Currency,CurrentPrice_Amount, CurrentPrice_Currency FROM Courses";

            DataSet dataSet = await DatabaseAccessor.QueryAsync(query);
            var dataTable = dataSet.Tables[0];

            var courseList = new List<CourseViewModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                CourseViewModel course = CourseViewModel.FromDataRow(row);
                courseList.Add(course);
            }
            return courseList;
        }
    }
}
