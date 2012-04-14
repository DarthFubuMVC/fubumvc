using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg.New;
using NUnit.Framework;
using FubuTestingSupport;
using FubuCore;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class WriterTester
    {
        [Test]
        public void create_from_closed_type()
        {
            var writer = new Writer(typeof (SpecificWriter));

            writer.ResourceType.ShouldEqual(typeof (OutputTarget));

            writer.WriterType.ShouldEqual(typeof (SpecificWriter));
        }

        [Test]
        public void create_object_def()
        {
            var writer = new Writer(typeof(SpecificWriter));
            writer.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)
                .FindDependencyDefinitionFor<IMediaWriter<OutputTarget>>()
                .Type.ShouldEqual(typeof (SpecificWriter));
        }

        [Test]
        public void create_from_open_type_without_resource_type_should_throw()
        {
            Exception<ArgumentNullException>.ShouldBeThrownBy(() =>
            {
                new Writer(typeof (GenericWriter<>));
            });
        }

        [Test]
        public void create_from_open_type_and_resource_type()
        {
            var writer = new Writer(typeof (GenericWriter<>), typeof (OutputTarget));

            writer.ResourceType.ShouldEqual(typeof (OutputTarget));
            writer.WriterType.ShouldEqual(typeof (GenericWriter<OutputTarget>));
        }

        [Test]
        public void create_object_def_by_starting_from_open_type_and_resource_type()
        {
            var writer = new Writer(typeof(GenericWriter<>), typeof(OutputTarget));

            writer.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)
                .FindDependencyDefinitionFor<IMediaWriter<OutputTarget>>()
                .Type.ShouldEqual(typeof(GenericWriter<OutputTarget>));
        }

        [Test]
        public void blow_up_if_the_type_is_not_a_media_writer()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new Writer(typeof (OutputTarget));
            });
        }
    }

    public class SpecificWriter : IMediaWriter<OutputTarget>
    {
        public void Write(string mimeType, OutputTarget resource)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Mimetypes
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class GenericWriter<T> : IMediaWriter<T>
    {
        public void Write(string mimeType, T resource)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Mimetypes
        {
            get { throw new NotImplementedException(); }
        }
    }
}