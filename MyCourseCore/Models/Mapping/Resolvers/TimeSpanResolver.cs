using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Mapping.Resolvers
{
    public class TimeSpanResolver : IValueResolver<DataRow, object, TimeSpan>
    {
        private readonly string memberName;
        public TimeSpanResolver(string memberName)
        {
            this.memberName = memberName;
        }
        public TimeSpan Resolve(DataRow source, object destination, TimeSpan destMember, ResolutionContext context)
        {
            return TimeSpan.Parse((string)source[memberName]);
        }
    }
}
