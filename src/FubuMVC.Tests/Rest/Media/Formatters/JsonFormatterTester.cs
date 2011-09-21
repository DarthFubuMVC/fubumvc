using System;
using System.IO;
using FubuMVC.Core.Rest.Media.Formatters;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Rest.Projections;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Rest.Media.Formatters
{
    [TestFixture]
    public class JsonFormatterTester : InteractionContext<JsonFormatter>
    {
        protected override void beforeEach()
        {
            MockFor<IStreamingData>().Stub(x => x.Output).Return(new MemoryStream());
        }

        [Test]
        public void has_the_right_mime_types()
        {
            ClassUnderTest.MatchingMimetypes.ShouldHaveTheSameElementsAs("application/json", "text/json");
        }

        [Test]
        public void writes_with_the_correct_mimetype_passed_into_it()
        {
            var theAddress = new Address();
            ClassUnderTest.Write(theAddress, "application/json");
            MockFor<IJsonWriter>().AssertWasCalled(x => x.Write(theAddress, "application/json"));
        }
    }

    [TestFixture]
    public class XmlFormatterTester : InteractionContext<XmlFormatter>
    {
        protected override void beforeEach()
        {
            MockFor<IStreamingData>().Stub(x => x.Output).Return(new MemoryStream());
        }

        [Test]
        public void has_the_right_mime_types()
        {
            ClassUnderTest.MatchingMimetypes.ShouldHaveTheSameElementsAs("text/xml", "application/xml");
        }

        [Test]
        public void writes_the_correct_mimetype_passed_into_it()
        {
            ClassUnderTest.Write(new Address(), "application/xml");
            MockFor<IStreamingData>().AssertWasCalled(x => x.OutputContentType = "application/xml");
        }
    }
}