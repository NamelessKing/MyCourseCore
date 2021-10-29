using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Mapping.Resolvers
{
    public class DefaultResolver : IValueResolver<DataRow, object, object>
    {
        private readonly string memberName;
        public DefaultResolver(string memberName)
        {
            this.memberName = memberName;
        }
        public object Resolve(DataRow source, object destination, object destMember, ResolutionContext context)
        {
            return source[memberName];
        }
    }
}
