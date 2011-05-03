using Bottles.Deployment.Writing;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace Bottles.Tests.Deployment.Writing
{
    [TestFixture]
    public class PropertyValueTester
    {
        [Test]
        public void to_string_with_host_name()
        {
            var value = PropertyValue.For<OneDirective>(x => x.Age, 23);
            value.HostName = "Host1";

            value.ToString().ShouldEqual("Host1.OneDirective.Age=23");
        }

        [Test]
        public void write_with_host_name()
        {
            var value = PropertyValue.For<OneDirective>(x => x.Age, 23);
            value.HostName = "Host1";

            var writer = MockRepository.GenerateMock<IFlatFileWriter>();

            value.Write(writer);

            writer.AssertWasCalled(x => x.WriteProperty("Host1.OneDirective.Age", "23"));
        }

        [Test]
        public void to_string_with_accessor_and_no_host()
        {
            var value = PropertyValue.For<OneDirective>(x => x.Age, 23);

            value.ToString().ShouldEqual("OneDirective.Age=23");
        }

        [Test]
        public void write_with_accessor_and_no_host()
        {
            var value = PropertyValue.For<OneDirective>(x => x.Age, 23);
            var writer = MockRepository.GenerateMock<IFlatFileWriter>();

            value.Write(writer);

            writer.AssertWasCalled(x => x.WriteProperty("OneDirective.Age", "23"));
        }

        [Test]
        public void to_string_with_name_but_no_accessor()
        {
            var value = new PropertyValue(){
                Name = "Key1",
                Value = "Value1"
            };

            value.ToString().ShouldEqual("Key1=Value1");
        }

        [Test]
        public void write_with_name_but_no_accessor()
        {
            var value = new PropertyValue()
            {
                Name = "Key1",
                Value = "Value1"
            };

            var writer = MockRepository.GenerateMock<IFlatFileWriter>();

            value.Write(writer);

            writer.AssertWasCalled(x => x.WriteProperty("Key1", "Value1"));
        }
    }
}