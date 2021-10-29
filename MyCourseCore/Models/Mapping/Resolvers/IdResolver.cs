using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Mapping.Resolvers
{
    public class IdResolver : IValueResolver<DataRow, object, int>
    {
        public int Resolve(DataRow source, object destination, int destMember, ResolutionContext context)
        {
            return Convert.ToInt32(source["Id"]);
        }
    }
}
