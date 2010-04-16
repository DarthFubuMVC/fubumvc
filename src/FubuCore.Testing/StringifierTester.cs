using System;
using FubuCore.Reflection;
using System.Reflection;
using NUnit.Framework;

namespace FubuCore.Testing
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

            var site = new FakeSite {Billing = address, Shipping = address};

            const string expectedDefaultFormatting = "2050 Ozark, Joplin";
            const string expectedOverrideFormatting = "Joplin-2050 Ozark";
            var billingRequest = new GetStringRequest(site, ReflectionHelper.GetProperty<FakeSite>(s => s.Billing), address);
            var shippingRequest = new GetStringRequest(site, ReflectionHelper.GetProperty<FakeSite>(s => s.Shipping), address);
            stringifier.GetStringRequest(billingRequest).ShouldEqual(expectedDefaultFormatting);
            stringifier.GetStringRequest(shippingRequest).ShouldEqual(expectedOverrideFormatting);
        }

        [Test]
        public void register_a_property_override_for_a_string_conversion_passing_property_info()
        {
            PropertyInfo passedProperty = null;

            //default formatting for Address objects
            stringifier.IfIsType<Address>(a => "{0}, {1}".ToFormat(a.Address1, a.City));
            
            //specific override formatting for Address objects named Shipping
            stringifier.IfPropertyMatches<Address>(
                prop => prop.Name == "Shipping", 
                (req, value) =>
                {
                    passedProperty = req.Property;
                    return "{1}-{0}".ToFormat(value.Address1, value.City);
                });

            var address = new Address();
            var site = new FakeSite { Billing = address, Shipping = address };

            var shippingRequest = new GetStringRequest(site, ReflectionHelper.GetProperty<FakeSite>(s => s.Shipping), address);

            stringifier.GetStringRequest(shippingRequest);

            passedProperty.Name.ShouldEqual("Shipping");
        }

        [Test]
        public void register_a_property_override_for_a_string_conversion_passing_original_model()
        {
            object passedModel = null;

            //default formatting for Address objects
            stringifier.IfIsType<Address>(a => "{0}, {1}".ToFormat(a.Address1, a.City));

            //specific override formatting for Address objects named Shipping
            stringifier.IfPropertyMatches<Address>(
                prop => prop.Name == "Shipping",
                (req, value) =>
                {
                    passedModel = req.Model;
                    return "{1}-{0}".ToFormat(value.Address1, value.City);
                });

            var address = new Address();
            var site = new FakeSite { Billing = address, Shipping = address };

            var shippingRequest = new GetStringRequest(site, ReflectionHelper.GetProperty<FakeSite>(s => s.Shipping), address);

            stringifier.GetStringRequest(shippingRequest);

            passedModel.ShouldBeTheSameAs(site);
        }

        [Test]
        public void register_a_property_override_for_a_string_conversion_passing_raw_value()
        {
            object passedRawValue = null;

            //default formatting for Address objects
            stringifier.IfIsType<Address>(a => "{0}, {1}".ToFormat(a.Address1, a.City));

            //specific override formatting for Address objects named Shipping
            stringifier.IfPropertyMatches<Address>(
                prop => prop.Name == "Shipping",
                (req, value) =>
                {
                    passedRawValue = req.RawValue;
                    return "{1}-{0}".ToFormat(value.Address1, value.City);
                });

            var address = new Address();
            var site = new FakeSite { Billing = address, Shipping = address };

            var shippingRequest = new GetStringRequest(site, ReflectionHelper.GetProperty<FakeSite>(s => s.Shipping), address);

            stringifier.GetStringRequest(shippingRequest);

            passedRawValue.ShouldBeTheSameAs(address);
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


    public class Address
    {
        public Address()
        {
            StateOrProvince = string.Empty;
            Country = string.Empty;
            AddressType = string.Empty;
        }

        public int Order { get; set; }

        public bool IsActive { get; set; }

        public string AddressType { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string StateOrProvince { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        //[Required]
        //public TimeZoneInfo TimeZone { get; set; }


        public DateTime DateEntered { get; set; }

        public Guid Guid { get; set; }
    }


}