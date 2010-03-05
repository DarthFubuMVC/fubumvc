using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Util;
using FubuMVC.UI;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class StringifierTester
    {
        [SetUp]
        public void SetUp()
        {
            stringifier = new Stringifier();
        }

        private Stringifier stringifier;

        public interface Something
        {
            string Description { get; }
        }

        public class RedSomething : Something
        {
            public string Description { get { return "Red"; } }
        }

        public class BlueSomething : Something
        {
            public string Description { get { return "Blue"; } }
        }

        public class FakeSite
        {
            public Address Shipping { get; set; }
            public Address Billing { get; set; }
        }

        [Test]
        public void get_date_value_out_of_the_box()
        {
            DateTime date = DateTime.Today;
            stringifier.GetString(date).ShouldEqual(date.ToString());
        }

        [Test]
        public void get_int_values_out_of_the_box()
        {
            stringifier.GetString(123).ShouldEqual(123);
        }

        [Test]
        public void get_string_values_out_of_the_box()
        {
            stringifier.GetString(null).ShouldBeEmpty();
            stringifier.GetString("a").ShouldEqual("a");
        }

        [Test]
        public void register_a_string_conversion_for_a_class()
        {
            stringifier.IfIsType<Address>(a => "{0}, {1}".ToFormat(a.Address1, a.City));

            var address = new Address
            {
                Address1 = "2050 Ozark",
                City = "Joplin"
            };

            stringifier.GetString(address).ShouldEqual("2050 Ozark, Joplin");
        }

        [Test]
        public void register_a_property_override_for_a_string_conversion()
        {
            //default formatting for Address objects
            stringifier.IfIsType<Address>(a => "{0}, {1}".ToFormat(a.Address1, a.City));
            //specific override formatting for Address objects named Shipping
            stringifier.IfPropertyMatches<Address>(prop => prop.Name == "Shipping", a => "{1}-{0}".ToFormat(a.Address1, a.City));

            var address = new Address
            {
                Address1 = "2050 Ozark",
                City = "Joplin"
            };

            const string expectedDefaultFormatting = "2050 Ozark, Joplin";
            const string expectedOverrideFormatting = "Joplin-2050 Ozark";
            stringifier.GetString(ReflectionHelper.GetProperty<FakeSite>(s => s.Billing), address).ShouldEqual(expectedDefaultFormatting);
            stringifier.GetString(ReflectionHelper.GetProperty<FakeSite>(s => s.Shipping), address).ShouldEqual(expectedOverrideFormatting);
        }


        [Test]
        public void register_a_string_conversion_for_a_series_of_types()
        {
            stringifier.IfCanBeCastToType<Something>(s => s.Description);

            stringifier.GetString(new RedSomething()).ShouldEqual("Red");
            stringifier.GetString(new BlueSomething()).ShouldEqual("Blue");
        }

        [Test]
        public void register_a_string_conversion_function_by_a_struct_type()
        {
            stringifier.IfIsType<DateTime>(d => d.ToShortDateString());

            DateTime date = DateTime.Today;
            stringifier.GetString(date).ShouldEqual(date.ToShortDateString());
        }

        [Test]
        public void string_conversion_functions_work_for_nullable_types()
        {
            stringifier.IfIsType<DateTime>(d => d.ToShortDateString());

            DateTime date = DateTime.Today;
            stringifier.GetString(date).ShouldEqual(date.ToShortDateString());
        }

        [Test]
        public void string_conversion_functions_work_for_nullable_types_that_are_null()
        {
            stringifier.IfIsType<DateTime>(d => d.ToShortDateString());

            stringifier.GetString(DateTime.Parse("17-JAN-2007")).ShouldEqual(DateTime.Parse("17-JAN-2007").ToShortDateString());
        }

        [Test]
        public void should_return_empty_when_no_converter_is_defined_for_type()
        {
            stringifier.GetString(null).ShouldEqual("");
        }

        [Test]
        public void should_return_empty_when_value_is_null()
        {
            stringifier.IfIsType<DateTime>(d => d.ToShortDateString());
            stringifier.GetString(null).ShouldEqual("");
        }
    }
}