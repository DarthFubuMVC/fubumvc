using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Resources.Media.Formatters;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class OutputNodeTester
    {
        [Test]
        public void add_formatter()
        {
            var node = new OutputNode(typeof (Address));
            node.AddFormatter<JsonFormatter>();

            node.Writers.Single()
                .ShouldEqual(new WriteWithFormatter(typeof (Address), typeof (JsonFormatter)));
                
        }

        [Test]
        public void add_writer_happy_path()
        {
            var node = new OutputNode(typeof(Address));
            var writer = node.AddWriter<FakeAddressWriter>();

            node.Writers.Single().ShouldBeTheSameAs(writer);

            writer.ResourceType.ShouldEqual(typeof (Address));
            writer.WriterType.ShouldEqual(typeof (FakeAddressWriter));
        }

    }
}