using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Json;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.Core.StructureMap;
using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.Resources.Conneg
{
    [TestFixture]
    public class OutputNodeTester : IMediaWriter<string>
    {
        [Test]
        public void ClearAll()
        {
            var node = new OutputNode(typeof (Address));
            node.Add(new XmlFormatter());
            node.Add(new NewtonsoftJsonFormatter());

            node.ClearAll();

            node.Explicits.Any().ShouldBeFalse();
        }

        public IEnumerable<string> Mimetypes { get; private set; }
        public void Write(string mimeType, IFubuRequestContext context, string resource)
        {
            throw new NotImplementedException();
        }


        [Test]
        public void implements_the_IMayHaveResourceType_interface()
        {
            var node = new OutputNode(typeof (Address));
            node.As<IMayHaveResourceType>().ResourceType().ShouldBe(node.ResourceType);
        }

        [Test]
        public void add_a_custom_no_resource_handler()
        {
            var node = new OutputNode(typeof (Address));
            node.UseForResourceNotFound<MyFakeResourceNotHandler>();

            var def = node.As<IContainerModel>().ToInstance().As<IConfiguredInstance>();

            def.FindDependencyDefinitionFor<IResourceNotFoundHandler>()
                .ReturnedType.ShouldBe(typeof (MyFakeResourceNotHandler));
        }

        [Test]
        public void no_custom_resource_not_found_handler()
        {
            var node = new OutputNode(typeof (Address));

            var def = node.As<IContainerModel>().ToInstance().As<IConfiguredInstance>();

            def.FindDependencyDefinitionFor<IResourceNotFoundHandler>()
                .ShouldBeNull();
        }

        [Test]
        public void add_writer_by_formatter_happy_path_no_condition()
        {
            var node = new OutputNode(typeof(Address));
            var theFormatter = new NewtonsoftJsonFormatter();
            node.Add(theFormatter);

            node.Explicits.Single().ShouldBeOfType<FormatterWriter<Address>>();

        }

        [Test]
        public void add_writer_with_explicit_condition()
        {
            var node = new OutputNode(typeof(Address));
            var theFormatter = new NewtonsoftJsonFormatter();
            node.Add(theFormatter);

            node.Explicits.Single().ShouldBeOfType<FormatterWriter<Address>>();

        }

        [Test]
        public void add_writer_happy_path_with_open_type()
        {
            var node = new OutputNode(typeof(Address)) {typeof (FooWriter<>)};

            node.Explicits.Single().ShouldBeOfType<FooWriter<Address>>();
        }

        [Test]
        public void add_writer_happy_path_with_open_type_and_explicit_condition()
        {
            var node = new OutputNode(typeof (Address));
            node.Add(typeof(FooWriter<>));

            node.Explicits.Single().ShouldBeOfType<FooWriter<Address>>();
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

            var media = node.Explicits.Single().ShouldBeTheSameAs(writer);
        }

        [Test]
        public void add_a_closed_writer_with_conditional()
        {
            var writer = new SpecialWriter();
            var node = new OutputNode(typeof(Address));
            node.Add(writer);

            node.Explicits.Single().ShouldBeTheSameAs(writer);
        }

        [Test]
        public void add_a_writer_by_object_sad_path()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new OutputNode(typeof(Address)).Add(this);
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