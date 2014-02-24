using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

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
        public void JsonOnly_from_scratch()
        {
            var node = new OutputNode(typeof (Address));
            node.JsonOnly();

            node.Writers.ShouldHaveCount(1);
            node.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void JsonOnly_with_existing_stuff()
        {
            var node = new OutputNode(typeof (Address));
            node.UsesFormatter<XmlFormatter>();

            node.JsonOnly();

            node.Writers.ShouldHaveCount(1);
            node.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void UsesFormatter()
        {
            var node = new OutputNode(typeof (Address));

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
        public void implements_the_IMayHaveResourceType_interface()
        {
            var node = new OutputNode(typeof (Address));
            node.As<IMayHaveResourceType>().ResourceType().ShouldEqual(node.ResourceType);
        }

        [Test]
        public void add_a_custom_no_resource_handler()
        {
            var node = new OutputNode(typeof (Address));
            node.UseForResourceNotFound<MyFakeResourceNotHandler>();

            var def = node.As<IContainerModel>().ToObjectDef();

            def.FindDependencyDefinitionFor<IResourceNotFoundHandler>()
                .Type.ShouldEqual(typeof (MyFakeResourceNotHandler));
        }

        [Test]
        public void no_custom_resource_not_found_handler()
        {
            var node = new OutputNode(typeof (Address));

            var def = node.As<IContainerModel>().ToObjectDef();

            def.DependencyFor<IResourceNotFoundHandler>()
                .ShouldBeNull();
        }

        [Test]
        public void add_writer_by_formatter_happy_path_no_condition()
        {
            var node = new OutputNode(typeof(Address));
            var theFormatter = new JsonSerializer();
            node.Add(theFormatter);

            var media = node.Media().Single().ShouldBeOfType<Media<Address>>();

            media.Writer.ShouldBeOfType<FormatterWriter<Address>>()
                .Formatter.ShouldBeTheSameAs(theFormatter);

            media.Condition.ShouldBeTheSameAs(Always.Flyweight);

        }

        [Test]
        public void add_writer_with_explicit_condition()
        {
            var node = new OutputNode(typeof(Address));
            var theFormatter = new JsonSerializer();
            var condition = new IsAjaxRequest();
            node.Add(theFormatter, condition);

            var media = node.Media().Single().ShouldBeOfType<Media<Address>>();

            media.Writer.ShouldBeOfType<FormatterWriter<Address>>()
                .Formatter.ShouldBeTheSameAs(theFormatter);

            media.Condition.ShouldBeTheSameAs(condition);

        }

        [Test]
        public void add_writer_happy_path_with_open_type()
        {
            var node = new OutputNode(typeof(Address)) {typeof (FooWriter<>)};

            var media = node.Media().Single().ShouldBeOfType<Media<Address>>();
            media.Writer.ShouldBeOfType<FooWriter<Address>>();
            media.Condition.ShouldBeTheSameAs(Always.Flyweight);
        }

        [Test]
        public void add_writer_happy_path_with_open_type_and_explicit_condition()
        {
            var condition = new IsAjaxRequest();
            var node = new OutputNode(typeof (Address));
            node.Add(typeof(FooWriter<>), condition);

            var media = node.Media().Single().ShouldBeOfType<Media<Address>>();
            media.Writer.ShouldBeOfType<FooWriter<Address>>();
            media.Condition.ShouldBeTheSameAs(condition);
        }

        [Test]
        public void add_writer_sad_path()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                var node = new OutputNode(typeof(Address));
                node.Add(GetType());
            });
        }

        [Test]
        public void add_a_closed_writer_happy_path()
        {
            var writer = new SpecialWriter();
            var node = new OutputNode(typeof (Address));
            node.Add(writer);

            var media = node.Media().Single().ShouldBeOfType<Media<Address>>();
            media.Writer.ShouldBeTheSameAs(writer);
            media.Condition.ShouldBeTheSameAs(Always.Flyweight);
        }

        [Test]
        public void add_a_closed_writer_with_conditional()
        {
            var condition = new IsAjaxRequest();
            var writer = new SpecialWriter();
            var node = new OutputNode(typeof(Address));
            node.Add(writer, condition);

            var media = node.Media().Single().ShouldBeOfType<Media<Address>>();
            media.Writer.ShouldBeTheSameAs(writer);
            media.Condition.ShouldBeTheSameAs(condition);
        }

        [Test]
        public void add_a_writer_by_object_sad_path()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new OutputNode(typeof(Address))
                    .Add(this);
            });
        }
    }

    public class FooWriter<T> : IMediaWriter<T>
    {
        public void Write(string mimeType, IFubuRequestContext context, T resource)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> Mimetypes { get; private set; }
    }

    public class SpecialWriter : IMediaWriter<Address>
    {
        public void Write(string mimeType, IFubuRequestContext context, Address resource)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> Mimetypes { get; private set; }
    }

    public class MyFakeResourceNotHandler : IResourceNotFoundHandler
    {
        public void HandleResourceNotFound<T>()
        {
        }
    }
}