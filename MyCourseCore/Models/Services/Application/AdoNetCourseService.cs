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


        public CourseDetailViewModel GetCourse(int id)
        {
            throw new NotImplementedException();
        }

        public List<CourseViewModel> GetCourses()
        {
            string query = "SELECT * FROM Courses";
            DataSet dataSet =  DatabaseAccessor.Query(query);
        }
    }
}
