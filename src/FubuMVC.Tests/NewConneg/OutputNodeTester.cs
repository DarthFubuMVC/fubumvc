using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Resources.Media.Formatters;
using HtmlTags;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class OutputNodeTester
    {
        [Test]
        public void ClearAll()
        {
            Assert.Fail("Do.");
        }


        [Test]
        public void JsonOnly_from_scratch()
        {
            Assert.Fail("Do.");
        }

        [Test]
        public void JsonOnly_with_existing_stuff()
        {
            Assert.Fail("Do.");
        }

        [Test]
        public void UsesFormatter()
        {
            Assert.Fail("Do.");
        }

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

        [Test]
        public void add_html_to_the_end()
        {
            var node = new OutputNode(typeof(HtmlTag));
            var html = node.AddHtml();

            node.Writers.Single().ShouldBeOfType<WriteHtml>()
                .ResourceType.ShouldEqual(typeof (HtmlTag));
        }

        [Test]
        public void add_html_to_the_end_is_idempotent()
        {
            var node = new OutputNode(typeof(HtmlTag));
            var html = node.AddHtml();
            node.AddHtml();
            node.AddHtml();
            node.AddHtml();
            node.AddHtml();


            node.Writers.Single().ShouldBeOfType<WriteHtml>()
                .ResourceType.ShouldEqual(typeof(HtmlTag));
        }

        [Test]
        public void adding_a_formatter_is_idempotent()
        {
            var node = new OutputNode(typeof(Address));
            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<JsonFormatter>();

            node.Writers.Single()
                .ShouldEqual(new WriteWithFormatter(typeof(Address), typeof(JsonFormatter)));
        }

    }
}