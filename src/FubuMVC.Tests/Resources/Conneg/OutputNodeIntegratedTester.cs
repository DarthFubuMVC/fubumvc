using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Json;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using Xunit;
using Rhino.Mocks;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.Resources.Conneg
{
    
    public class OutputNodeIntegratedTester
    {
        private OutputBehavior<Address> theInputBehavior;

        public OutputNodeIntegratedTester()
        {
            var node = new OutputNode(typeof(Address));
            node.Add(new NewtonsoftJsonFormatter());
            node.Add(new XmlFormatter());
            node.Add(new FakeAddressWriter());

            using (var runtime = FubuRuntime.Basic())
            {
                var container = runtime.Get<IContainer>();

                container.Configure(x =>
                {
                    // Need a stand in value
                    x.For<IHttpRequest>().Use(MockRepository.GenerateMock<IHttpRequest>());
                });

                theInputBehavior =
                    container.GetInstance<OutputBehavior<Address>>(node.As<IContainerModel>().ToInstance());
            }
        }


        [Fact]
        public void first_off_the_behavior_can_be_built()
        {
            theInputBehavior.ShouldNotBeNull();
        }

        [Fact]
        public void first_media_has_the_formatter()
        {
            theInputBehavior.Media.First()
                .ShouldBeOfType<FormatterWriter<Address>>();
        }

        [Fact]
        public void second_media_has_the_xml_formatter()
        {
            theInputBehavior.Media.ElementAt(1).ShouldBeOfType<FormatterWriter<Address>>();
        }

        [Fact]
        public void third_media_is_the_specific_writer_with_the_conditional()
        {
            theInputBehavior.Media.ElementAt(2)
                .ShouldBeOfType<FakeAddressWriter>();
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