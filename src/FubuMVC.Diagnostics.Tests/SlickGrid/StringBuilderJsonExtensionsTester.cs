using System.Text;
using FubuMVC.Diagnostics.SlickGrid;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.SlickGrid
{
    [TestFixture]
    public class StringBuilderJsonExtensionsTester
    {
        [Test]
        public void write_formatter()
        {
            var builder = new StringBuilder();

            builder.WriteJsonProp("key", SlickGridFormatter.TypeFormatter);
            builder.ToString().ShouldEqual("key: " + SlickGridFormatter.TypeFormatter.Name);
        }

        [Test]
        public void write_number()
        {
            var builder = new StringBuilder();

            builder.WriteJsonProp("key", 1);
            builder.ToString().ShouldEqual("key: 1");
        }

        [Test]
        public void write_string()
        {
            var builder = new StringBuilder();

            builder.WriteJsonProp("key", "1");
            builder.ToString().ShouldEqual("key: \"1\"");
        }

        [Test]
        public void write_boolean_true()
        {
            var builder = new StringBuilder();

            builder.WriteJsonProp("key", true);
            builder.ToString().ShouldEqual("key: true");
        }

        [Test]
        public void write_boolean_false()
        {
            var builder = new StringBuilder();

            builder.WriteJsonProp("key", true);
            builder.ToString().ShouldEqual("key: true");
        }

    }
}