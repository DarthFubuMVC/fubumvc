using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Resources.Media.Formatters;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class WriteWithFormatterTester
    {
        [Test]
        public void finds_mime_types_off_of_the_formatter_type()
        {
            var node = new Core.Resources.Conneg.New.WriteWithFormatter(typeof(Address), typeof(SomeFormatter));

            node.Mimetypes.ShouldHaveTheSameElementsAs("text/html", "other/mimetype");
        }

        [Test]
        public void if_a_formatter_type_does_not_expose_mimetypes_just_say_unknown()
        {
            new Core.Resources.Conneg.New.WriteWithFormatter(typeof(Address), typeof(UnknownFormatter))
            .Mimetypes.ShouldHaveTheSameElementsAs("Unknown");
        }

        [Test]
        public void build_object_def()
        {
            var node = new Core.Resources.Conneg.New.WriteWithFormatter( typeof(Address), typeof(SomeFormatter));
            var objectDef = node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);
            objectDef.FindDependencyDefinitionFor<IMediaWriter<Address>>()
                .Type.ShouldEqual(typeof(FormatterWriter<Address, SomeFormatter>));

            
        }

        [Test]
        public void argument_null_on_resource_type()
        {
            Exception<ArgumentNullException>.ShouldBeThrownBy(() =>
            {
                new Core.Resources.Conneg.New.WriteWithFormatter(null, typeof(JsonFormatter));
            });
        }

        [Test]
        public void argument_out_of_range_if_formatter_type__is_not_IFormatter()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new Core.Resources.Conneg.New.WriteWithFormatter(typeof(Address), GetType());
            });
        }
    }
}