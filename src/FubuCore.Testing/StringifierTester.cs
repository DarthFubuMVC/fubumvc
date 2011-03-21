using System;
using FubuCore.Reflection;
using System.Reflection;
using FubuMVC.StructureMap;
using NUnit.Framework;
using StructureMap;

namespace FubuCore.Testing
{
    [TestFixture]
    public class StringifierTester
    {
        [SetUp]
        public void SetUp()
        {
            stringifier = new Stringifier();
            container = new Container();
            locator = new StructureMapServiceLocator(container);
        }

        private Stringifier stringifier;
        private StructureMapServiceLocator locator;
        private Container container;

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

        private void configure(Action<DisplayConversionRegistry> configure)
        {
            var registry = new DisplayConversionRegistry();
            configure(registry);

            registry.Configure(stringifier);
        }

        [Test]
        public void register_a_string_conversion_for_a_class()
        {
            configure(x => x.IfIsType<Address>().ConvertBy(a => "{0}, {1}".ToFormat(a.Address1, a.City)));

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
            configure(x =>
            {
                //specific override formatting for Address objects named Shipping
                x.IfPropertyMatches<Address>(prop => prop.Name == "Shipping").ConvertBy(a => "{1}-{0}".ToFormat(a.Address1, a.City));

                //default formatting for Address objects
                x.IfIsType<Address>().ConvertBy(a => "{0}, {1}".ToFormat(a.Address1, a.City));
            });

            var address = new Address
            {
                Address1 = "2050 Ozark",
                City = "Joplin"
            };

            const string expectedDefaultFormatting = "2050 Ozark, Joplin";
            const string expectedOverrideFormatting = "Joplin-2050 Ozark";
            var billingRequest = new GetStringRequest(ReflectionHelper.GetAccessor<FakeSite>(s => s.Billing), address, locator);
            var shippingRequest = new GetStringRequest(ReflectionHelper.GetAccessor<FakeSite>(s => s.Shipping), address, locator);
            stringifier.GetString(billingRequest).ShouldEqual(expectedDefaultFormatting);
            stringifier.GetString(shippingRequest).ShouldEqual(expectedOverrideFormatting);
        }

        [Test]
        public void register_a_property_override_for_a_string_conversion_passing_property_info()
        {
            PropertyInfo passedProperty = null;

            configure(x =>
            {
                //specific override formatting for Address objects named Shipping
                x.IfPropertyMatches<Address>(prop => prop.Name == "Shipping")
                    .ConvertBy((req, value) =>
                    {
                        passedProperty = req.Property;
                        return "{1}-{0}".ToFormat(value.Address1, value.City);
                    });
                
                //default formatting for Address objects
                x.IfIsType<Address>().ConvertBy(a => "{0}, {1}".ToFormat(a.Address1, a.City));


            });

            var address = new Address();

            var shippingRequest = new GetStringRequest(ReflectionHelper.GetAccessor<FakeSite>(s => s.Shipping), address, locator);

            stringifier.GetString(shippingRequest);

            passedProperty.Name.ShouldEqual("Shipping");
        }


        [Test]
        public void register_a_property_override_for_a_string_conversion_passing_raw_value()
        {
            object passedRawValue = null;

            configure(x =>
            {
                //specific override formatting for Address objects named Shipping
                x.IfPropertyMatches<Address>(prop => prop.Name == "Shipping").ConvertBy((req, value) =>
                {
                    passedRawValue = req.RawValue;
                    return "{1}-{0}".ToFormat(value.Address1, value.City);
                });
                
                //default formatting for Address objects
                x.IfIsType<Address>().ConvertBy(a => "{0}, {1}".ToFormat(a.Address1, a.City));


            });

            var address = new Address();

            var shippingRequest = new GetStringRequest(ReflectionHelper.GetAccessor<FakeSite>(s => s.Shipping), address, locator);

            stringifier.GetString(shippingRequest);

            passedRawValue.ShouldBeTheSameAs(address);
        }


        [Test]
        public void register_a_string_conversion_for_a_series_of_types()
        {
            configure(x => x.IfCanBeCastToType<Something>().ConvertBy(s => s.Description));

            stringifier.GetString(new RedSomething()).ShouldEqual("Red");
            stringifier.GetString(new BlueSomething()).ShouldEqual("Blue");
        }

        [Test]
        public void register_a_string_conversion_function_by_a_struct_type()
        {
            configure(x => x.IfIsType<DateTime>().ConvertBy(d => d.ToShortDateString()));

       
            DateTime date = DateTime.Today;
            stringifier.GetString(date).ShouldEqual(date.ToShortDateString());
        }

        [Test]
        public void string_conversion_functions_work_for_nullable_types()
        {
            configure(x => x.IfIsType<DateTime>().ConvertBy(d => d.ToShortDateString()));

            DateTime date = DateTime.Today;
            stringifier.GetString(date).ShouldEqual(date.ToShortDateString());
        }

        [Test]
        public void string_conversion_functions_work_for_nullable_types_that_are_null()
        {
            configure(x => x.IfIsType<DateTime>().ConvertBy(d => d.ToShortDateString()));

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
            configure(x => x.IfIsType<DateTime>().ConvertBy(d => d.ToShortDateString()));
            stringifier.GetString(null).ShouldEqual("");
        }

        [Test]
        public void stringifier_can_use_a_service_to_get_at_a_display()
        {
            container.Configure(x => x.For<IWidgetDisplayer>().Use<WidgetDisplayer>());

            configure(x =>
            {
                x.IfCanBeCastToType<Widget>().ConvertBy((r, w) => r.Get<IWidgetDisplayer>().ToDisplay(w));
            });

            var widget = new Widget
            {
                Color = "Red"
            };

            var request = new GetStringRequest(null, widget, locator);

            stringifier.GetString(request).ShouldEqual("A Red widget");
        }
    }

    public class Widget
    {
        public string Color { get; set; }
    }

    public interface IWidgetDisplayer
    {
        string ToDisplay(Widget widget);
    }

    public class WidgetDisplayer : IWidgetDisplayer
    {
        public string ToDisplay(Widget widget)
        {
            return "A {0} widget".ToFormat(widget.Color);
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