using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.StructureMap;
using NUnit.Framework;
using Rhino.Mocks;
using OutputNode = FubuMVC.Core.Resources.Conneg.OutputNode;
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
            node.Add(new JsonSerializer());
            node.Add(new XmlFormatter());
            node.Add(new FakeAddressWriter(), new SomeConditional());

            var container = StructureMapContainerFacility.GetBasicFubuContainer();
            container.Configure(x =>
            {
                // Need a stand in value
                x.For<IHttpRequest>().Use(MockRepository.GenerateMock<IHttpRequest>());
            });

            var objectDef = node.As<IContainerModel>().ToObjectDef();
            var instance = new ObjectDefInstance(objectDef);

            theInputBehavior = container.GetInstance<OutputBehavior<Address>>(instance);
        }

        [Test]
        public void first_off_the_behavior_can_be_built()
        {
            theInputBehavior.ShouldNotBeNull();
        }

        [Test]
        public void first_media_has_the_formatter()
        {
            theInputBehavior.Media.First()
                .ShouldBeOfType<Media<Address>>()
                .Writer.ShouldBeOfType<FormatterWriter<Address>>();

        }

        [Test]
        public void second_media_has_the_xml_formatter()
        {
            theInputBehavior.Media.ElementAt(1)
               .ShouldBeOfType<Media<Address>>()
               .Writer.ShouldBeOfType<FormatterWriter<Address>>();

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
        public bool ShouldExecute(IFubuRequestContext context)
        {
            return true;
        }
    }

    public class FancyWriter<T> : IMediaWriter<T>
    {
        public void Write(string mimeType, IFubuRequestContext context, T resource)
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
        public void Write(string mimeType, IFubuRequestContext context, Address resource)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return "fake/address"; }
        }
    }
}