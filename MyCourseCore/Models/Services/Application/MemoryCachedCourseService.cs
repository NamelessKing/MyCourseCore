using Microsoft.Extensions.Caching.Memory;
using MyCourseCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Services.Application
{
    public class MemoryCachedCourseService : ICachedCourseService
    {
        public MemoryCachedCourseService(ICourseService courseService, IMemoryCache memoryCache)
        {
            CourseService = courseService;
            MemoryCache = memoryCache;
        }

        private ICourseService CourseService { get; }
        private IMemoryCache MemoryCache { get; }

        public Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            return MemoryCache.GetOrCreateAsync($"Course{id}", cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return CourseService.GetCourseAsync(id);
            });
        }

        public Task<List<CourseViewModel>> GetCoursesAsync()
        {
            return MemoryCache.GetOrCreateAsync($"Course", cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return CourseService.GetCoursesAsync();
            });
        }
    }
}
