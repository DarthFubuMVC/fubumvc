using Bottles.Deployment;
using Bottles.Deployment.Writing;
using FubuCore.Reflection;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests.Deployment.Writing
{
    [TestFixture]
    public class HostWriterTester
    {
        private HostWriter theWriter;

        [SetUp]
        public void SetUp()
        {
            theWriter = new HostWriter(new TypeDescriptorCache());    
        }

        [Test]
        public void write_bottle_reference_with_no_relationship()
        {
            theWriter.WriteReference(new BottleReference(){
                Name = "Bottle1"
            });

            theWriter.ToText().Trim().ShouldEqual("bottle:Bottle1");
        }

        [Test]
        public void write_bottle_reference_with_a_relationship()
        {
            theWriter.WriteReference(new BottleReference(){
                Name = "webcore"
            });

            theWriter.ToText().Trim().ShouldEqual("bottle:webcore");
        }

        [Test]
        public void write_simple_settings_with_values_for_everything()
        {
            var settings = new SimpleSettings(){
                One = "one",
                Two = "two"
            };

            theWriter.WriteDirective(settings);

            theWriter.AllLines().ShouldHaveTheSameElementsAs("SimpleSettings.One=one", "SimpleSettings.Two=two");
        }

        [Test]
        public void write_simple_settings_with_values_for_everything_and_bottle_ref()
        {
            var settings = new SimpleSettings()
            {
                One = "one",
                Two = "two"
            };

            theWriter.WriteDirective(settings);
            theWriter.WriteReference(new BottleReference("bob"));

            theWriter.AllLines().ShouldHaveTheSameElementsAs("SimpleSettings.One=one", "SimpleSettings.Two=two", "bottle:bob");
        }

        [Test]
        public void write_simple_settings_with_a_null_value()
        {
            var settings = new SimpleSettings()
            {
                One = "one",
                Two = null
            };

            theWriter.WriteDirective(settings);
            theWriter.AllLines().ShouldHaveTheSameElementsAs("SimpleSettings.One=one", "SimpleSettings.Two=");
        }

        [Test]
        public void do_not_write_complex_setting_if_the_prop_is_null()
        {
            var settings = new ComplexSettings();

            theWriter.WriteDirective(settings);

            theWriter.AllLines().ShouldHaveTheSameElementsAs(
                "ComplexSettings.Name=",
                "ComplexSettings.Flag=False"
                );
        }

        [Test]
        public void do_write_complex_setting_when_the_prop_is_not_null()
        {
            var settings = new ComplexSettings(){
                Simple = new SimpleSettings(){
                    One = "one"
                }
            };

            theWriter.WriteDirective(settings);

            theWriter.AllLines().ShouldHaveTheSameElementsAs(
                "ComplexSettings.Name=",
                "ComplexSettings.Flag=False",
                "ComplexSettings.Simple.One=one",
                "ComplexSettings.Simple.Two="
                );
        }

        [Test]
        public void write_simple_property_value()
        {
            var value = PropertyValue.For<SimpleSettings>(x => x.One, "one");
            theWriter.WritePropertyValue(value);

            theWriter.AllLines().ShouldHaveTheSameElementsAs("SimpleSettings.One=one");
        }

        [Test]
        public void write_complex_property_value()
        {
            var value = PropertyValue.For<ComplexSettings>(x => x.Simple.One, "one");
            theWriter.WritePropertyValue(value);

            theWriter.AllLines().ShouldHaveTheSameElementsAs("ComplexSettings.Simple.One=one");
        }
    }

    public class SimpleSettings : IDirective
    {
        public string One { get; set; }
        public string Two { get; set; }
    }

    public class ComplexSettings : IDirective
    {
        public string Name { get; set; }
        public bool Flag { get; set; }
        public SimpleSettings Simple { get; set; }
    }
}