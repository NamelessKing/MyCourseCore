using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
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

        public Task<List<CourseViewModel>> GetCoursesAsync(string search, int page)
        {
            return MemoryCache.GetOrCreateAsync($"Courses{search}-{page}", cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return CourseService.GetCoursesAsync(search,page);
            });
        }
    }
}
