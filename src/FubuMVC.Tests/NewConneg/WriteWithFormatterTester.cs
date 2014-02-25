using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class WriteWithFormatterTester
    {
        [Test]
        public void argument_null_on_resource_type()
        {
            Exception<ArgumentNullException>.ShouldBeThrownBy(
                () => { new WriteWithFormatter(null, typeof (JsonFormatter)); });
        }

        [Test]
        public void argument_out_of_range_if_formatter_type__is_not_IFormatter()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                () => { new WriteWithFormatter(typeof (Address), GetType()); });
        }

        [Test]
        public void build_object_def()
        {
            var node = new WriteWithFormatter(typeof (Address), typeof (SomeFormatter));
            var objectDef = node.As<IContainerModel>().ToObjectDef();
            objectDef.FindDependencyDefinitionFor<IMediaWriter<Address>>()
                .Type.ShouldEqual(typeof (FormatterWriter<Address, SomeFormatter>));
        }


        [Test]
        public void if_a_formatter_type_does_not_expose_mimetypes_just_say_unknown()
        {
            new WriteWithFormatter(typeof (Address), typeof (UnknownFormatter))
                .Mimetypes.ShouldHaveTheSameElementsAs("Unknown");
        }
    }

    public class SomeFormatter : IFormatter
    {
        public IEnumerable<string> MatchingMimetypes
        {
            get
            {
                return new[] {"text/html", "other/mimetype"};
            }
        }

        public void Write<T>(IFubuRequestContext context, T target, string mimeType)
        {
            throw new NotImplementedException();
        }

        public T Read<T>(IFubuRequestContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class UnknownFormatter : IFormatter
    {
        public IEnumerable<string> MatchingMimetypes
        {
            get
            {
                yield break;
            }
        }
        public void Write<T>(IFubuRequestContext context, T target, string mimeType)
        {
            throw new NotImplementedException();
        }

        public T Read<T>(IFubuRequestContext context)
        {
            throw new NotImplementedException();
        }
    }


}