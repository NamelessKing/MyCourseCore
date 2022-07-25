using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyCourseCore.Models.InputModels;
using MyCourseCore.Models.Options;
using MyCourseCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Services.Application
{
    public class MemoryCachedCourseService : ICachedCourseService
    {
        public MemoryCachedCourseService(ICourseService courseService, IMemoryCache memoryCache, IOptionsMonitor<CoursesOptions> coursesOptions)
        {
            CourseService = courseService;
            MemoryCache = memoryCache;
            CoursesOptions = coursesOptions;
        }

        private ICourseService CourseService { get; }
        private IMemoryCache MemoryCache { get; }
        private IOptionsMonitor<CoursesOptions> CoursesOptions { get; }

        public Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            return MemoryCache.GetOrCreateAsync($"Course{id}", cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(CoursesOptions.CurrentValue.CourseCacheTimeInSec));
                return CourseService.GetCourseAsync(id);
            });
        }

        public Task<List<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {
            //Metto in cache i risultati solo per le prime 5 pagine del catalogo, che reputo essere
            //le più visitate dagli utenti, e che perciò mi permettono di avere il maggior beneficio dalla cache.
            //E inoltre, metto in cache i risultati solo se l'utente non ha cercato nulla.
            //In questo modo riduco drasticamente il consumo di memoria RAM
            bool canCache = model.Page <= 5 && string.IsNullOrEmpty(model.Search);

            //Se canCache è true, sfrutto il meccanismo di caching
            if (canCache)
            {
                return MemoryCache.GetOrCreateAsync($"Courses{model.Page}-{model.OrderBy}-{model.Ascending}", cacheEntry =>
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                    return CourseService.GetCoursesAsync(model);
                });
            }

            //Altrimenti uso il servizio applicativo sottostante, che recupererà sempre i valori dal database
            return CourseService.GetCoursesAsync(model);
        }
    }
}
