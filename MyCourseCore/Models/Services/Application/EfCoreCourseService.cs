using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourseCore.Models.Entities;
using MyCourseCore.Models.Exceptions;
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
            if (viewModel == null)
            {
                Logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }
            //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione
            //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione
            //.SingleAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto o contiene più di un elemento, solleva un'eccezione

            return viewModel;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync(string search, int page)
        {
            search = search ?? "";
            page = Math.Max(1, page);
            int limit = CoursesOptions.CurrentValue.PerPage;
            int offset = (page - 1) * limit;
            IQueryable<CourseViewModel> queryLinq = DbContext.Courses
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
