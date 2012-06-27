using System;
using System.Linq.Expressions;
using FubuCore.Dates;
using FubuCore.Formatting;
using FubuMVC.Core;
using FubuMVC.Core.Runtime.Dates;
using NUnit.Framework;
using StructureMap;
using FubuMVC.StructureMap;
using FubuCore.Reflection;
using FubuTestingSupport;

namespace FubuMVC.Tests.Runtime.Dates
{
    [TestFixture]
    public class DateTimeFormatting_import_integration_testing
    {
        private IDisplayFormatter _formatter;
        private TimeZoneInfo theTimeZone;
        private SimpleTimeZoneContext theTimeZoneContext;
        private DateTime localTime;
        private DateTime utcTime;
        private Container container;

        [SetUp]
        public void SetUp()
        {
            _formatter = null;
            theTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            theTimeZoneContext = new SimpleTimeZoneContext(theTimeZone);

            localTime = new DateTime(2012, 6, 27, 8, 0, 0);
            utcTime = localTime.ToUniversalTime(theTimeZone);
        }

        private void configurationIs(Action<DateTimePolicies> action)
        {
            container = new Container();
            var registry = new FubuRegistry();
            registry.Services(x =>
            {
                x.SetServiceIfNone<ITimeZoneContext>(theTimeZoneContext);
            });
            registry.Import<DateTimePolicies>(action);

            FubuApplication.For(registry).StructureMap(container).Bootstrap();


            _formatter = container.GetInstance<IDisplayFormatter>();
        }

        public string format(Expression<Func<DateTimeFormattingTarget, object>> expression, object value)
        {
            var accessor = expression.ToAccessor();

            return _formatter.GetDisplayForValue(accessor, value);
        }

        [Test]
        public void default_short_date_formatting()
        {
            configurationIs(x => { }); // defaults

            format(x => x.NullableDate, utcTime).ShouldEqual(localTime.ToString("d"));
            format(x => x.SomeDate, utcTime).ShouldEqual(localTime.ToString("d"));
        }

        [Test]
        public void default_long_time_formatting()
        {
            configurationIs(x => { }); // defaults

            format(x => x.NullableTime, utcTime).ShouldEqual(localTime.ToString("f"));
            format(x => x.FullTime, utcTime).ShouldEqual(localTime.ToString("f"));
        }

        [Test]
        public void default_timestamp_formatter()
        {
            configurationIs(x => { }); // defaults

            format(x => x.Timestamp, utcTime).ShouldEqual(localTime.ToString("F"));
        }

        [Test]
        public void fallback_formatter()
        {
            configurationIs(x => { }); // defaults

            format(x => x.Fallback, utcTime).ShouldEqual(localTime.ToString("F"));
        }

        [Test]
        public void default_short_date_formatting_without_utc()
        {
            configurationIs(x =>
            {
                x.UtcDateHandling = UtcDateHandling.NoConversion;
            }); // defaults

            format(x => x.NullableDate, utcTime).ShouldEqual(utcTime.ToString("d"));
            format(x => x.SomeDate, utcTime).ShouldEqual(utcTime.ToString("d"));
        }

        [Test]
        public void default_long_time_formatting_without_utc()
        {
            configurationIs(x =>
            {
                x.UtcDateHandling = UtcDateHandling.NoConversion;
            }); // defaults

            format(x => x.NullableTime, utcTime).ShouldEqual(utcTime.ToString("f"));
            format(x => x.FullTime, utcTime).ShouldEqual(utcTime.ToString("f"));
        }

        [Test]
        public void default_timestamp_formatter_without_utc()
        {
            configurationIs(x =>
            {
                x.UtcDateHandling = UtcDateHandling.NoConversion;
            }); // defaults

            format(x => x.Timestamp, utcTime).ShouldEqual(utcTime.ToString("F"));
        }

        [Test]
        public void fallback_formatter_without_utc()
        {
            configurationIs(x =>
            {
                x.UtcDateHandling = UtcDateHandling.NoConversion;
            }); // defaults

            format(x => x.Fallback, utcTime).ShouldEqual(utcTime.ToString("F"));
        }

        [Test]
        public void change_the_default_format()
        {
            configurationIs(x => x.DefaultDateTimeFormat = "MM/dd/yyyy");

            format(x => x.Fallback, utcTime).ShouldEqual(utcTime.ToString("MM/dd/yyyy"));
        }

        [Test]
        public void change_the_rules()
        {
            configurationIs(x =>
            {
                x.FormatAs("g").IfThePropertyNameMatches(name => name.Contains("Nullable"));
            });

            format(x => x.NullableDate, utcTime).ShouldEqual(localTime.ToString("g"));
            format(x => x.NullableTime, utcTime).ShouldEqual(localTime.ToString("g"));
        
            // Still falls thru to defaults
            format(x => x.FullTime, utcTime).ShouldEqual(localTime.ToString("f"));
            format(x => x.SomeDate, utcTime).ShouldEqual(localTime.ToString("d"));
        }

        [Test]
        public void can_register_a_time_zone_context()
        {
            configurationIs(x => x.TimeZoneContext<FakeTimeZoneContext>());

            container.GetInstance<ITimeZoneContext>().ShouldBeOfType<FakeTimeZoneContext>();    
        }
    }

    public class FakeTimeZoneContext : ITimeZoneContext
    {
        public TimeZoneInfo GetTimeZone()
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimeFormattingTarget
    {
        public DateTime? NullableDate { get; set; }
        public DateTime? NullableTime { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime FullTime { get; set; }
        public DateTime SomeDate { get; set; }

        public DateTime Fallback { get; set; }
    }
}