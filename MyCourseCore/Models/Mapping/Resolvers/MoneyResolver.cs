using AutoMapper;
using MyCourseCore.Models.Enums;
using MyCourseCore.Models.ValueTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Mapping.Resolvers
{
    public class MoneyResolver : IValueResolver<DataRow, object, Money>
    {
        private readonly string prefix;
        public MoneyResolver(string prefix)
        {
            this.prefix = prefix;
        }
        public Money Resolve(DataRow source, object destination, Money destMember, ResolutionContext context)
        {
            string currencyColumnName = $"{prefix}_Currency";
            string amountColumnName = $"{prefix}_Amount";

            Currency currency = Enum.Parse<Currency>(System.Convert.ToString(source[currencyColumnName]));
            decimal amount = System.Convert.ToDecimal(source[amountColumnName]);

            return new Money(currency, amount);
        }
    }
}
