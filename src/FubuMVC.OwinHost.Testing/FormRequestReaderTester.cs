using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class FormRequestReaderTester
    {
        [Test]
        public void can_parse_query_string_with_encoding()
        {
            FormRequestReader.Parse("Anesth=Moore%2C+Roy")["Anesth"].ShouldEqual("Moore, Roy");
            
        }

        [Test]
        public void can_parse_field_values_in_query_string()
        {
            FormRequestReader.Parse("Moore%2C+Roy=Anesth")["Moore, Roy"].ShouldEqual("Anesth");
        }

        [Test]
        public void can_parse_multiple_values()
        {
            var dict = FormRequestReader.Parse("a=1&b=2&c=3");

            dict["a"].ShouldEqual("1");
            dict["b"].ShouldEqual("2");
            dict["c"].ShouldEqual("3");

            dict.Count.ShouldEqual(3);
        }
    }
}