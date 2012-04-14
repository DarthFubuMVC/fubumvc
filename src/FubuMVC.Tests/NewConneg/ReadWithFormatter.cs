using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Resources.Media.Formatters;
using NUnit.Framework;
using FubuTestingSupport;
using FubuCore;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class ReadWithFormatter
    {
        [Test]
        public void finds_mime_types_off_of_the_formatter_type()
        {
            var node = new Core.Resources.Conneg.New.ReadWithFormatter(typeof(Address), typeof (SomeFormatter));
            
            node.Mimetypes.ShouldHaveTheSameElementsAs("text/html", "other/mimetype");
        }

        [Test]
        public void if_a_formatter_type_does_not_expose_mimetypes_just_say_unknown()
        {
            new Core.Resources.Conneg.New.ReadWithFormatter(typeof(Address), typeof(UnknownFormatter))
            .Mimetypes.ShouldHaveTheSameElementsAs("Unknown");
        }

        [Test]
        public void build_object_def()
        {
            var node = new Core.Resources.Conneg.New.ReadWithFormatter(typeof(Address), typeof(SomeFormatter));
            node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)
                .Type.ShouldEqual(typeof (FormatterReader<Address, SomeFormatter>));
        }

        [Test]
        public void argument_null_on_resource_type()
        {
            Exception<ArgumentNullException>.ShouldBeThrownBy(() =>
            {
                new Core.Resources.Conneg.New.ReadWithFormatter(null, typeof (JsonFormatter));
            });
        }

        [Test]
        public void argument_out_of_range_if_formatter_type__is_not_IFormatter()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new Core.Resources.Conneg.New.ReadWithFormatter(typeof (Address), GetType());
            });
        }
    }

    [MimeType("text/html", "other/mimetype")]
    public class SomeFormatter : IFormatter
    {
        public IEnumerable<string> MatchingMimetypes
        {
            get { throw new NotImplementedException(); }
        }

        public void Write<T>(T target, string mimeType)
        {
            throw new NotImplementedException();
        }

        public T Read<T>()
        {
            throw new NotImplementedException();
        }
    }

    public class UnknownFormatter : IFormatter
    {
        public IEnumerable<string> MatchingMimetypes
        {
            get { throw new NotImplementedException(); }
        }

        public void Write<T>(T target, string mimeType)
        {
            throw new NotImplementedException();
        }

        public T Read<T>()
        {
            throw new NotImplementedException();
        }
    }
}