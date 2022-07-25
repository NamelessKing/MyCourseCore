using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourseCore.Models.Entities;
using MyCourseCore.Models.Exceptions;
using MyCourseCore.Models.InputModels;
using MyCourseCore.Models.Options;
using MyCourseCore.Models.Services.Infrastructure;
using MyCourseCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Services.Application
{
    public class EfCoreCourseService : ICourseService
    {
        public EfCoreCourseService(MyCourseDbContext dbContext, ILogger<EfCoreCourseService> logger, IOptionsMonitor<CoursesOptions> coursesOptions)
        {
            DbContext = dbContext;
            Logger = logger;
            CoursesOptions = coursesOptions;
        }

        private MyCourseDbContext DbContext { get; }
        public ILogger<EfCoreCourseService> Logger { get; }
        private IOptionsMonitor<CoursesOptions> CoursesOptions { get; }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            IQueryable<CourseDetailViewModel> queryLinq = DbContext.Courses.AsNoTracking()
                .Include(course => course.Lessons)
                .Where(course => course.Id == id)
                .Select(course => CourseDetailViewModel.FromEntity(course)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            CourseDetailViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
            //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione
            //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione
            //.SingleAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto o contiene più di un elemento, solleva un'eccezione

            if (viewModel == null)
            {
                Logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync(CourseListInputModel courseListInputModel)
        {
            var search = courseListInputModel.Search ?? "";
            var page = Math.Max(1, courseListInputModel.Page);
            int limit = CoursesOptions.CurrentValue.PerPage;
            int offset = (page - 1) * limit;
            var orderby = "";
            bool ascending = false;

            var orderOptions = CoursesOptions.CurrentValue.Order;
            if (!orderOptions.Allow.Contains(courseListInputModel.OrderBy))
            {
                orderby = orderOptions.By;
                ascending = orderOptions.Ascending;
            }

            IQueryable<Course> baseQuery = DbContext.Courses;

            switch (orderby)
            {
                case "Title":
                    if (ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Title);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Title);
                    }
                    break;
                case "Rating":
                    if (ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Rating);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Rating);
                    }
                    break;
                case "CurrentPrice":
                    if (ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.CurrentPrice.Amount);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.CurrentPrice.Amount);
                    }
                    break;
                case "Id":
                    if (ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Id);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Id);
                    }
                    break;
            }

            IQueryable<CourseViewModel> queryLinq = baseQuery
                .Where(course => course.Title.Contains(search))
                .Skip(offset)
                .Take(limit)
                .AsNoTracking()
                .Select(course => CourseViewModel.FromEntity(course)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            List<CourseViewModel> courses = await queryLinq.ToListAsync(); //La query al database viene inviata qui, quando manifestiamo l'intenzione di voler leggere i risultati

            return courses;
        }
    }
}
