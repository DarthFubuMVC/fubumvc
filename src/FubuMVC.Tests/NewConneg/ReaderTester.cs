using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Resources.Media;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class ReaderTester
    {
        [Test]
        public void create_from_closed_type()
        {
            var reader = new Reader(typeof(SpecificReader));

            reader.InputType.ShouldEqual(typeof(InputTarget));

            reader.ReaderType.ShouldEqual(typeof(SpecificReader));
        }

        [Test]
        public void create_object_def()
        {
            var reader = new Reader(typeof(SpecificReader));
            reader.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)
                          .Type.ShouldEqual(typeof(SpecificReader));
        }

        [Test]
        public void create_from_open_type_without_resource_type_should_throw()
        {
            Exception<ArgumentNullException>.ShouldBeThrownBy(() =>
            {
                new Reader(typeof(GenericReader<>));
            });
        }

        [Test]
        public void create_from_open_type_and_resource_type()
        {
            var reader = new Reader(typeof(GenericReader<>), typeof(InputTarget));

            reader.InputType.ShouldEqual(typeof(InputTarget));
            reader.ReaderType.ShouldEqual(typeof(GenericReader<InputTarget>));
        }

        [Test]
        public void create_object_def_by_starting_from_open_type_and_resource_type()
        {
            var reader = new Reader(typeof(GenericReader<>), typeof(InputTarget));

            reader.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)
                          .Type
                          .ShouldEqual(typeof(GenericReader<InputTarget>));
        }

        [Test]
        public void blow_up_if_the_type_is_not_a_media_Reader()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new Reader(typeof(InputTarget));
            });
        }

        [Test]
        public void reads_mime_types_from_the_attributes_of_the_reader()
        {
            var reader = new Reader(typeof(SpecificReader));
            reader.Mimetypes.ShouldHaveTheSameElementsAs("text/xml", "text/json");
        }
    }
}