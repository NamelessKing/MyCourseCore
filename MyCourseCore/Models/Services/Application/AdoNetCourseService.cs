using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourseCore.Models.Exceptions;
using MyCourseCore.Models.Options;
using MyCourseCore.Models.Services.Infrastructure;
using MyCourseCore.Models.ValueTypes;
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
        private ILogger<AdoNetCourseService> Logger { get; }
        private IDatabaseAccessor DatabaseAccessor { get; }
        private IOptionsMonitor<CoursesOptions> CoursesOptions { get; }
        private IMapper Mapper { get; }

        public AdoNetCourseService(ILogger<AdoNetCourseService> logger, IDatabaseAccessor databaseAccessor, IOptionsMonitor<CoursesOptions> coursesOptions, IMapper mapper)
        {
            Logger = logger;
            DatabaseAccessor = databaseAccessor;
            CoursesOptions = coursesOptions;
            Mapper = mapper;
        }


        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {

            Logger.LogInformation("Course {id} requested", id);

            FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, 
                            CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Id={id};
                            SELECT Id, Title, Description, Duration FROM Lessons WHERE CourseId={id}";

            DataSet dataSet = await DatabaseAccessor.QueryAsync(query);

            //Course
            var courseTable = dataSet.Tables[0];
            if (courseTable.Rows.Count != 1)
            {
                Logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }
            var courseRow = courseTable.Rows[0];
            var courseDetailViewModel = Mapper.Map<CourseDetailViewModel>(courseRow);

            //Course lessons
            var lessonDataTable = dataSet.Tables[1];

            courseDetailViewModel.Lessons = Mapper.Map<List<LessonViewModel>>(lessonDataTable.Rows);

            return courseDetailViewModel;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync(string search, int page, string orderby, bool ascending)
        {
            //Sanitizzazione
            page = Math.Max(1, page);
            int limit = CoursesOptions.CurrentValue.PerPage;
            int offset = (page - 1)* limit;

            var orderOptions = CoursesOptions.CurrentValue.Order;
            if (!orderOptions.Allow.Contains(orderby))
            {
                orderby = orderOptions.By;
                ascending = orderOptions.Ascending;
            }

            //Cosa estrarre dal DB
            if (orderby == "CurrentPrice")
            {
                orderby = "CurrentPrice_Amount";
            }
            string direction = ascending ? "ASC" : "DESC";


            FormattableString query = 
                $@"SELECT Id, Title, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency,CurrentPrice_Amount, CurrentPrice_Currency 
                FROM Courses
                WHERE Title LIKE %{search}%
                ORDER BY {(Sql) orderby} {(Sql) direction}
                LIMIT {limit} 
                OFFSET {offset}";

            DataSet dataSet = await DatabaseAccessor.QueryAsync(query);
            var dataTable = dataSet.Tables[0];

            var courseList = Mapper.Map<List<CourseViewModel>>(dataTable.Rows);

            return courseList;
        }
    }
}
