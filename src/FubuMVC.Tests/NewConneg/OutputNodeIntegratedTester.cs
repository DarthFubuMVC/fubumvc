using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.StructureMap;
using NUnit.Framework;
using Rhino.Mocks;
using OutputNode = FubuMVC.Core.Resources.Conneg.New.OutputNode;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.NewConneg
{

    [TestFixture]
    public class OutputNodeIntegratedTester
    {
        private OutputBehavior<Address> theInputBehavior;

        #region Setup/Teardown

        [TestFixtureSetUp]
        public void SetUp()
        {
            var node = new OutputNode(typeof (Address));
            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<XmlFormatter>();
            node.AddWriter<FakeAddressWriter>().Condition<SomeConditional>();
            

            var container = StructureMapContainerFacility.GetBasicFubuContainer();
            container.Configure(x =>
            {
                // Need a stand in value
                x.For<IStreamingData>().Use(MockRepository.GenerateMock<IStreamingData>());
            });

            var objectDef = node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);
            var instance = new ObjectDefInstance(objectDef);

            theInputBehavior = container.GetInstance<OutputBehavior<Address>>(instance);
        }

        [Test]
        public void first_off_the_behavior_can_be_built()
        {
            theInputBehavior.ShouldNotBeNull();
        }

        [Test]
        public void has_two_media()
        {
            theInputBehavior.Media.Count().ShouldEqual(3);
        }


        [Test]
        public void first_media_has_the_formatter()
        {
            theInputBehavior.Media.First()
                .ShouldBeOfType<Media<Address>>()
                .Writer.ShouldBeOfType<FormatterWriter<Address, JsonFormatter>>();

        }

        [Test]
        public void second_media_has_the_xml_formatter()
        {
            theInputBehavior.Media.ElementAt(1)
               .ShouldBeOfType<Media<Address>>()
               .Writer.ShouldBeOfType<FormatterWriter<Address, XmlFormatter>>();

        }

        [Test]
        public void third_media_is_the_specific_writer_with_the_conditional()
        {
            var media = theInputBehavior.Media.ElementAt(2)
                .ShouldBeOfType<Media<Address>>();

            media.Writer.ShouldBeOfType<FakeAddressWriter>();
            media.Condition.ShouldBeOfType<SomeConditional>();
        }

        #endregion
    }

    public class SomeConditional : IConditional
    {
        public bool ShouldExecute()
        {
            return true;
        }
    }

    public class FancyWriter<T> : IMediaWriter<T>
    {
        public void Write(string mimeType, T resource)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return "fancy/writer"; }
        }
    }

    public class FakeAddressWriter : IMediaWriter<Address>
    {
        public void Write(string mimeType, Address resource)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return "fake/address"; }
        }
    }
}