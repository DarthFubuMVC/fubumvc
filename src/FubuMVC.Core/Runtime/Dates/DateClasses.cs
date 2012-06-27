using System;
using System.ComponentModel;
using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Dates;

namespace FubuMVC.Core.Runtime.Dates
{
    [Description("Ensures that all dates sent from the client are converted to UTC dates")]
    public class DateTimeToUtcConverterFamily : StatelessConverter
    {
        public override bool Matches(PropertyInfo property)
        {
            return property.PropertyType.IsDateTime();
        }

        public override object Convert(IPropertyContext context)
        {
            if (context.Property.PropertyType.IsNullable() && isEmpty(context.RawValueFromRequest.RawValue)) return null;

            var userTimeZone = context.Get<ITimeZoneContext>().GetTimeZone();
            var localTime = System.Convert.ToDateTime(context.ValueAs<DateTime>());

            return localTime.ToUniversalTime(userTimeZone);
        }

        private static bool isEmpty(object value)
        {
            return value == null || value.ToString().IsEmpty();
        }
    }
}