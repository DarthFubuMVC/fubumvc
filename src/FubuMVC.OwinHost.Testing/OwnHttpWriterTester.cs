using System.Collections.Generic;
using System.IO;
using System.Net;
using NUnit.Framework;
using FubuTestingSupport;
using FubuCore;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class OwnHttpWriterTester
    {
        private IDictionary<string, object> environment;
        private OwinHttpWriter writer;

        [SetUp]
        protected void beforeEach()
        {
            environment = new Dictionary<string, object>();
            environment.Add(OwinConstants.ResponseBodyKey, new MemoryStream());

            writer = new OwinHttpWriter(environment);
        }

        [Test]
        public void can_write_multiple_values_for_the_same_header()
        {
            writer.AppendHeader("X-1", "A");
            writer.AppendHeader("X-1", "B");

            var dictionary = environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey);
            dictionary
                .Get("X-1").ShouldHaveTheSameElementsAs("A", "B");
        }

        [Test]
        public void should_set_response_code()
        {
            writer.WriteResponseCode(HttpStatusCode.UseProxy);

            environment[OwinConstants.ResponseStatusCodeKey].ShouldEqual(HttpStatusCode.UseProxy.As<int>());
        }

        [Test]
        public void should_set_response_code_and_description()
        {
            const string description = "why u no make good request?";
            writer.WriteResponseCode(HttpStatusCode.BadRequest, description);
            environment[OwinConstants.ResponseStatusCodeKey].ShouldEqual(HttpStatusCode.BadRequest.As<int>());
            environment[OwinConstants.ResponseReasonPhraseKey].ShouldEqual(description);
        }
    }
}