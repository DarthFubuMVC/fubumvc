using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FubuMVC.StructureMap;
using NUnit.Framework;
using StructureMap;
using System.Linq;

namespace FubuCore.Testing
{
    public class Service
    {
        private readonly string _name;

        public Service(string name)
        {
            _name = name;
        }

        public string Name { get { return _name; } }

        public bool Equals(Service obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Service)) return false;
            return Equals((Service)obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }
    }

    [TestFixture]
    public class ObjectConverterTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            finder = new ObjectConverter();
        }

        #endregion

        private ObjectConverter finder;

        [Test]
        public void array_of_non_simple_type_that_does_not_have_a_finder_cannot_be_parsed()
        {
            finder.CanBeParsed(typeof(Service[])).ShouldBeFalse();
        }

        [Test]
        public void array_of_non_simple_type_that_has_a_finder_can_be_parsed()
        {
            finder.RegisterConverter(x => new Service(x));
            finder.CanBeParsed(typeof(Service[])).ShouldBeTrue();
        }

        [Test]
        public void enumerable_of_non_simple_type_that_does_not_have_a_finder_cannot_be_parsed()
        {
            finder.CanBeParsed(typeof(IEnumerable<Service>)).ShouldBeFalse();
        }

        [Test]
        public void time_zone_info()
        {
            finder.FromString<TimeZoneInfo>("Eastern Standard Time").Id.ShouldEqual("Eastern Standard Time");
        }

        [Test]
        public void enumerable_of_non_simple_type_that_has_a_finder_can_be_parsed()
        {
            finder.RegisterConverter(x => new Service(x));
            finder.CanBeParsed(typeof(IEnumerable<Service>)).ShouldBeTrue();
        }

        [Test]
        public void enumerable_of_non_simple_type_that_has_a_finder_can_be_resolved()
        {
            finder.RegisterConverter(x => new Service(x));
            finder.FromString<IEnumerable<Service>>("Josh, Chad, Jeremy, Brandon").ShouldHaveTheSameElementsAs(new[]
            {
                new Service("Josh"), 
                new Service("Chad"), 
                new Service("Jeremy"), 
                new Service("Brandon")
            });
        }


        [Test]
        public void datetimes_can_be_parsed()
        {
            finder.CanBeParsed(typeof(DateTime)).ShouldBeTrue();
            finder.CanBeParsed(typeof(DateTime?)).ShouldBeTrue();
        }

        [Test]
        public void enumeration_can_be_parsed()
        {
            finder.CanBeParsed(typeof(DayOfWeek)).ShouldBeTrue();
        }

        [Test]
        public void get_date_time_for_day_and_time()
        {
            DateTime date = ObjectConverter.GetDateTime("Saturday 14:30");

            date.DayOfWeek.ShouldEqual(DayOfWeek.Saturday);
            date.Date.AddHours(14).AddMinutes(30).ShouldEqual(date);
            (date >= DateTime.Today).ShouldBeTrue();
        }

        [Test]
        public void get_date_time_for_day_and_time_2()
        {
            DateTime date = ObjectConverter.GetDateTime("Monday 14:30");

            date.DayOfWeek.ShouldEqual(DayOfWeek.Monday);
            date.Date.AddHours(14).AddMinutes(30).ShouldEqual(date);
            (date >= DateTime.Today).ShouldBeTrue();
        }

        [Test]
        public void get_date_time_for_day_and_time_3()
        {
            DateTime date = ObjectConverter.GetDateTime("Wednesday 14:30");

            date.DayOfWeek.ShouldEqual(DayOfWeek.Wednesday);
            date.Date.AddHours(14).AddMinutes(30).ShouldEqual(date);
            (date >= DateTime.Today).ShouldBeTrue();
        }

        [Test]
        public void get_date_time_from_24_hour_time()
        {
            ObjectConverter.GetDateTime("14:30").ShouldEqual(DateTime.Today.AddHours(14).AddMinutes(30));
        }

        [Test]
        public void get_EMPTY_as_an_empty_array()
        {
            finder.FromString<int[]>(ObjectConverter.EMPTY)
                .ShouldBeOfType<int[]>()
                .Length.ShouldEqual(0);

            finder.FromString<string[]>(ObjectConverter.EMPTY)
                .ShouldBeOfType<string[]>()
                .Length.ShouldEqual(0);
        }

        [Test]
        public void get_enumeration_value()
        {
            finder.FromString<DayOfWeek>(DayOfWeek.Monday.ToString())
                .ShouldEqual(DayOfWeek.Monday);
        }

        [Test]
        public void get_nullable_enumeration_value()
        {
            finder.FromString<DayOfWeek?>(DayOfWeek.Monday.ToString())
                .ShouldEqual(DayOfWeek.Monday);
            finder.FromString<DayOfWeek?>("NULL").ShouldBeNull();
        }

        [Test]
        public void get_string_enumerable()
        {
            finder.FromString<IEnumerable<string>>("Josh, Chad, Jeremy, Brandon").ShouldHaveTheSameElementsAs(
                new[] { "Josh", "Chad", "Jeremy", "Brandon" });
        }

        [Test]
        public void get_string_array()
        {
            finder.FromString<string[]>("Josh, Chad, Jeremy, Brandon")
                .ShouldEqual(new[] { "Josh", "Chad", "Jeremy", "Brandon" });
        }

        [Test]
        public void non_simple_type_that_does_not_have_a_finder_cannot_be_parsed()
        {
            finder.CanBeParsed(typeof(Service)).ShouldBeFalse();
        }

        [Test]
        public void non_simple_type_that_has_a_finder_can_be_parsed()
        {
            finder.RegisterConverter(x => new Service(x));
            finder.CanBeParsed(typeof(Service)).ShouldBeTrue();
        }

        [Test]
        public void nullable_enumeration_can_be_parsed()
        {
            finder.CanBeParsed(typeof(DayOfWeek?)).ShouldBeTrue();
        }

        [Test]
        public void nullables_can_be_parsed()
        {
            finder.CanBeParsed(typeof(int?)).ShouldBeTrue();
            finder.CanBeParsed(typeof(bool?)).ShouldBeTrue();
            finder.CanBeParsed(typeof(double?)).ShouldBeTrue();
        }

        [Test]
        public void parse_a_boolean()
        {
            finder.FromString<bool>("true").ShouldBeTrue();
            finder.FromString<bool>("True").ShouldBeTrue();
            finder.FromString<bool>("False").ShouldBeFalse();
            finder.FromString<bool>("false").ShouldBeFalse();
        }

        [Test]
        public void parse_a_nullable_boolean()
        {
            finder.FromString<bool?>("true").Value.ShouldBeTrue();
            finder.FromString<bool?>("True").Value.ShouldBeTrue();
            finder.FromString<bool?>("False").Value.ShouldBeFalse();
            finder.FromString<bool?>("false").Value.ShouldBeFalse();
            finder.FromString<bool?>("NULL").ShouldBeNull();
        }

        [Test]
        public void parsing_a_nullable_should_use_the_finder_for_the_inner_type()
        {
            finder.RegisterConverter<int>(text => 99);

            finder.FromString<int>("45").ShouldEqual(99);
            finder.FromString<int?>("32").ShouldEqual(99);
            finder.FromString<int?>("NULL").ShouldBeNull();
            finder.FromString<DateTime?>("TODAY").ShouldEqual(DateTime.Today);
        }

        [Test]
        public void parse_a_number()
        {
            finder.FromString<int>("32").ShouldEqual(32);
            finder.FromString<int>("-1").ShouldEqual(-1);

            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
                finder.FromString<double>("123.45").ShouldEqual(123.45);
        }

        [Test]
        public void parse_a_string()
        {
            finder.FromString<string>("something").ShouldEqual("something");
            finder.FromString<string>(ObjectConverter.BLANK).ShouldEqual(string.Empty);
            finder.FromString<string>(ObjectConverter.NULL).ShouldBeNull();
        }

        [Test]
        public void parse_a_null_string()
        {
            finder.FromString(null, typeof (string)).ShouldBeNull();
        }

        [Test]
        public void parse_an_empty_string()
        {
            finder.FromString("", typeof (string)).ShouldEqual("");
        }

        [Test]
        public void parse_a_nullable_int()
        {
            finder.FromString(null, typeof (int?)).ShouldBeNull();
        }

        [Test]
        public void empty_string_not_allowed_for_nullable_value_types()
        {
            //NOTE: We may want to change this in the future to have empty string = null, or default(T)
            typeof (Exception)
                .ShouldBeThrownBy(() => finder.FromString("", typeof (int?)));
        }

        [Test]
        public void parse_date()
        {
            finder.FromString<DateTime>("1/1/2009").ShouldEqual(new DateTime(2009, 1, 1));
        }

        [Test]
        public void parse_nullable_date()
        {
            finder.FromString<DateTime>("TODAY").ShouldEqual(DateTime.Today);
        }

        [Test]
        public void parse_timespan_as_days()
        {
            finder.FromString<TimeSpan>("5d").ShouldEqual(new TimeSpan(5, 0, 0, 0));
        }

        [Test]
        public void parse_timespan_as_hours()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
                finder.FromString<TimeSpan>("1.5 hours").ShouldEqual(new TimeSpan(1, 30, 0));
        }

        [Test]
        public void parse_timespan_as_minutes()
        {
            finder.FromString<TimeSpan>("  1   minute ").ShouldEqual(new TimeSpan(0, 1, 0));
        }

        [Test]
        public void parse_timespan_as_seconds()
        {
            finder.FromString<TimeSpan>("15 seconds").ShouldEqual(new TimeSpan(0, 0, 15));
        }

        [Test]
        public void parse_timespan_in_dotnet_format()
        {
            finder.FromString<TimeSpan>("3:5:2").ShouldEqual(new TimeSpan(3, 5, 2));
        }

        [Test]
        public void parse_timespan_with_unrecognized_units()
        {
            typeof(ApplicationException).ShouldBeThrownBy(() => finder.FromString<TimeSpan>("3 dais"));
        }

        [Test]
        public void parse_today()
        {
            finder.FromString<DateTime>("TODAY").ShouldEqual(DateTime.Today);
        }

        [Test]
        public void parse_today_minus_date()
        {
            finder.FromString<DateTime>("TODAY-3").ShouldEqual(DateTime.Today.AddDays(-3));
        }

        [Test]
        public void parse_today_plus_date()
        {
            finder.FromString<DateTime>("TODAY+5").ShouldEqual(DateTime.Today.AddDays(5));
        }

        [Test]
        public void primitive_array_can_be_parsed()
        {
            finder.CanBeParsed(typeof(string[])).ShouldBeTrue();
            finder.CanBeParsed(typeof(int[])).ShouldBeTrue();
            finder.CanBeParsed(typeof(bool?[])).ShouldBeTrue();
        }

        [Test]
        public void primitives_can_be_parsed()
        {
            finder.CanBeParsed(typeof(string)).ShouldBeTrue();
            finder.CanBeParsed(typeof(int)).ShouldBeTrue();
            finder.CanBeParsed(typeof(bool)).ShouldBeTrue();
        }

        [Test]
        public void register_and_retrieve_from_a_custom_finder_method()
        {
            finder.RegisterConverter(x => new Service(x));
            finder.FromString<Service>("Josh").Name.ShouldEqual("Josh");
        }

        [Test]
        public void register_and_retrieve_from_a_custom_finder_method_as_array()
        {
            finder.RegisterConverter(x => new Service(x));
            finder.FromString<Service[]>("Josh, Chad, Jeremy, Brandon")
                .ShouldEqual(new[]
                {
                    new Service("Josh"),
                    new Service("Chad"),
                    new Service("Jeremy"),
                    new Service("Brandon")
                });
        }

        [Test]
        public void a_service_can_not_be_parsed()
        {
            finder.CanBeParsed(typeof(ObjectConverter)).ShouldBeFalse();
        }

        [Test]
        public void register_and_retrieve_a_new_type_of_complex_object()
        {
            var finder = new ObjectConverter();
            finder.RegisterConverter<Contact>(text =>
            {
                var parts = text.Split(' ');
                return new Contact(){
                    FirstName = parts[0],
                    LastName = parts[1]
                };
            });

            var c = finder.FromString<Contact>("Jeremy Miller");

            c.FirstName.ShouldEqual("Jeremy");
            c.LastName.ShouldEqual("Miller");
        }

        [Test]
        public void how_about_getting_an_array_of_those_complex_objects()
        {
            // Same converter as before
            var finder = new ObjectConverter();
            finder.RegisterConverter<Contact>(text =>
            {
                var parts = text.Split(' ');
                return new Contact()
                {
                    FirstName = parts[0],
                    LastName = parts[1]
                };
            });

            // Now, let's pull an array of Contact's
            var contacts = 
                finder.FromString<Contact[]>("Jeremy Miller, Rod Paddock, Chad Myers");
        
            contacts.Select(x => x.LastName)
                .ShouldHaveTheSameElementsAs("Miller", "Paddock", "Myers");
        }
    }

    public class ScopedCulture : IDisposable
    {
        private readonly CultureInfo savedCulture;
        public ScopedCulture(CultureInfo culture)
        {
            savedCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = culture;
        }
        #region IDisposable Members
        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = savedCulture;
        }
        #endregion
    }

    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }



    [TestFixture]
    public class ServiceEnabledObjectConverterTester
    {
        private ServiceEnabledObjectConverter finder;

        [SetUp]
        public void SetUp()
        {
            var container = new Container();
            var locator = new StructureMapServiceLocator(container);

            finder = new ServiceEnabledObjectConverter(locator);
        }

        [Test]
        public void can_register_and_use_a_service_for_the_conversion()
        {
            finder.RegisterConverter<Widget, WidgetFinderService>((service, text) => service.Build(text));

            finder.FromString<Widget>("red").ShouldBeOfType<Widget>().Color.ShouldEqual("red");
        }
    }

    public class WidgetFinderService
    {
        public Widget Build(string text)
        {
            return new Widget(){Color = text};
        }
    }
}