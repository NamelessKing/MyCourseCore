﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Options
{
    public partial class CoursesOptions
    {
        public int PerPage { get; set; }
        public CoursesOrderOptions Order { get; set; }
        public int CourseCacheTimeInSec { get; set; }
    }

    public partial class CoursesOrderOptions
    {
        public string By { get; set; }
        public bool Ascending { get; set; }
        public string[] Allow { get; set; }
    }
}
