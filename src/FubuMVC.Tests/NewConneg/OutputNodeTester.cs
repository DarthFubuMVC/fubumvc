using System.Linq;
using FubuCore.DependencyAnalysis;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;
using FubuCore;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class OutputNodeTester
    {
        [Test]
        public void ClearAll()
        {
            var node = new OutputNode(typeof (Address));
            node.UsesFormatter<XmlFormatter>();
            node.UsesFormatter<JsonFormatter>();
        
            node.ClearAll();

            node.Writers.Any().ShouldBeFalse();
        }


        [Test]
        public void implements_the_IMayHaveResourceType_interface()
        {
            var node = new OutputNode(typeof(Address));
            node.As<IMayHaveResourceType>().ResourceType().ShouldEqual(node.ResourceType);
        }

        [Test]
        public void JsonOnly_from_scratch()
        {
            var node = new OutputNode(typeof(Address));
            node.JsonOnly();

            node.Writers.ShouldHaveCount(1);
            node.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void JsonOnly_with_existing_stuff()
        {
            var node = new OutputNode(typeof(Address));
            node.UsesFormatter<XmlFormatter>();

            node.JsonOnly();

            node.Writers.ShouldHaveCount(1);
            node.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void UsesFormatter()
        {
            var node = new OutputNode(typeof(Address));

            node.UsesFormatter<XmlFormatter>().ShouldBeFalse();
            node.UsesFormatter<JsonFormatter>().ShouldBeFalse();

            node.AddFormatter<XmlFormatter>();

            node.UsesFormatter<XmlFormatter>().ShouldBeTrue();
            node.UsesFormatter<JsonFormatter>().ShouldBeFalse();

            node.AddFormatter<JsonFormatter>();

            node.UsesFormatter<XmlFormatter>().ShouldBeTrue();
            node.UsesFormatter<JsonFormatter>().ShouldBeTrue();
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
        public void add_html_to_the_end()
        {
            var node = new OutputNode(typeof (HtmlTag));
            var html = node.AddHtml();

            node.Writers.Single().ShouldBeOfType<WriteHtml>()
                .ResourceType.ShouldEqual(typeof (HtmlTag));
        }

        [Test]
        public void add_html_to_the_end_is_idempotent()
        {
            var node = new OutputNode(typeof (HtmlTag));
            var html = node.AddHtml();
            node.AddHtml();
            node.AddHtml();
            node.AddHtml();
            node.AddHtml();


            node.Writers.Single().ShouldBeOfType<WriteHtml>()
                .ResourceType.ShouldEqual(typeof (HtmlTag));
        }

        [Test]
        public void add_writer_happy_path()
        {
            var node = new OutputNode(typeof (Address));
            var writer = node.AddWriter<FakeAddressWriter>();

            node.Writers.Single().ShouldBeTheSameAs(writer);

            writer.ResourceType.ShouldEqual(typeof (Address));
            writer.WriterType.ShouldEqual(typeof (FakeAddressWriter));
        }

        [Test]
        public void adding_a_formatter_is_idempotent()
        {
            var node = new OutputNode(typeof (Address));
            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<JsonFormatter>();

            node.Writers.Single()
                .ShouldEqual(new WriteWithFormatter(typeof (Address), typeof (JsonFormatter)));
        }

        [Test]
        public void add_view()
        {
            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<IViewToken>();

            var viewNode = node.AddView(viewToken);
            node.Writers.ShouldContain(viewNode);

            viewNode.View.ShouldBeTheSameAs(viewToken);
        }

        // Sometimes, ugly code just isn't worth the effort to fix it.
        [Test]
        public void add_view_with_condition()
        {
            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<IViewToken>();
            viewToken.Stub(x => x.ViewModel).Return(typeof (Address));
            viewToken.Stub(x => x.ToViewFactoryObjectDef()).Return(
                ObjectDef.ForValue(MockRepository.GenerateMock<IViewFactory>()));

            var viewNode = node.AddView(viewToken, typeof(FakeConditional));

            viewNode.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)
                .FindDependencyDefinitionFor<IConditional>()
                .Type
                .ShouldEqual(typeof (FakeConditional));
        }

        [Test]
        public void has_view_is_false_when_it_is_empty()
        {
            var node = new OutputNode(typeof(Address));
            node.HasView(typeof (Always)).ShouldBeFalse();
        }

        [Test]
        public void has_view_is_positive_with_always()
        {
            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<IViewToken>();
            node.AddView(viewToken);

            node.HasView(typeof(Always)).ShouldBeTrue();
        }

        [Test]
        public void has_view_positive_with_different_conditional()
        {
            var conditionType = typeof (FakeConditional);

            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<IViewToken>();
            node.AddView(viewToken, conditionType);

            node.HasView(conditionType).ShouldBeTrue();
        }

        [Test]
        public void has_view_negative_when_there_is_a_view_but_it_has_different_conditions()
        {
            var conditionType = typeof(FakeConditional);

            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<IViewToken>();
            node.AddView(viewToken, conditionType);

            node.HasView(typeof(Always)).ShouldBeFalse();
        }


    }
}