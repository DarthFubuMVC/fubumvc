using System;
using System.Diagnostics;
using FubuCore.Binding.InMemory;
using FubuCore.Dates;
using FubuMVC.Core.Runtime.Dates;
using NUnit.Framework;
using System.Collections.Generic;
using FubuTestingSupport;

namespace FubuMVC.Tests.Runtime.Dates
{
    [TestFixture]
    public class DateTimeToUtcConverterFamilyTester
    {

        [Test]
        public void simple_happy_path_for_a_date()
        {
            var localTime = new DateTime(2012, 6, 27, 8, 0, 0);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            BindingScenario<DateTimeTarget>.For(x =>
            {
                x.Registry.Add(new DateTimeToUtcConverterFamily());
                x.Service<ITimeZoneContext>(new SimpleTimeZoneContext(timeZone));

                x.Data(o => o.Date, localTime);
            }).Model.Date.ShouldEqual(localTime.ToUniversalTime(timeZone));
        }

        [Test]
        public void simple_happy_path_for_a_date_as_a_string()
        {
            var localTime = new DateTime(2012, 6, 27, 8, 0, 0);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            BindingScenario<DateTimeTarget>.For(x =>
            {
                x.Registry.Add(new DateTimeToUtcConverterFamily());
                x.Service<ITimeZoneContext>(new SimpleTimeZoneContext(timeZone));

                x.Data(o => o.Date, localTime.ToString());
            }).Model.Date.ShouldEqual(localTime.ToUniversalTime(timeZone));
        }

        [Test]
        public void nullable_property_has_no_data_and_should_be_null()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            BindingScenario<DateTimeTarget>.For(x =>
            {
                x.Registry.Add(new DateTimeToUtcConverterFamily());
                x.Service<ITimeZoneContext>(new SimpleTimeZoneContext(timeZone));

                // no data for NullableDate
            }).Model.NullableDate.ShouldBeNull();
        }

        [Test]
        public void nullable_property_has_empty_string_in_the_request()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            BindingScenario<DateTimeTarget>.For(x =>
            {
                x.Registry.Add(new DateTimeToUtcConverterFamily());
                x.Service<ITimeZoneContext>(new SimpleTimeZoneContext(timeZone));

                x.Data(o => o.NullableDate, "");
            }).Model.NullableDate.ShouldBeNull();
        }

        [Test]
        public void happy_path_for_nullable_datetime()
        {
            var localTime = new DateTime(2012, 6, 27, 8, 0, 0);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            BindingScenario<DateTimeTarget>.For(x =>
            {
                x.Registry.Add(new DateTimeToUtcConverterFamily());
                x.Service<ITimeZoneContext>(new SimpleTimeZoneContext(timeZone));

                x.Data(o => o.NullableDate, localTime.ToString());
            }).Model.NullableDate.ShouldEqual(localTime.ToUniversalTime(timeZone));
        }
    }

    public class DateTimeTarget
    {
        public DateTime Date { get; set; }
        public DateTime? NullableDate { get; set; }
    }
}