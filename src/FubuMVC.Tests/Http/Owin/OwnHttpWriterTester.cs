using System.Collections.Generic;
using System.IO;
using System.Net;
using FubuCore;
using FubuMVC.Core.Http.Owin;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class OwnHttpWriterTester
    {
        private IDictionary<string, object> environment;
        private OwinHttpResponse response;

        [SetUp]
        protected void beforeEach()
        {
            environment = new Dictionary<string, object>();
            environment.Add(OwinConstants.ResponseBodyKey, new MemoryStream());

            response = new OwinHttpResponse(environment);
        }

        [Test]
        public void can_write_multiple_values_for_the_same_header()
        {
            response.AppendHeader("X-1", "A");
            response.AppendHeader("X-1", "B");

            var dictionary = environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey);
            dictionary
                .Get("X-1").ShouldHaveTheSameElementsAs("A", "B");
        }

        [Test]
        public void should_set_response_code()
        {
            response.WriteResponseCode(HttpStatusCode.UseProxy);

            environment[OwinConstants.ResponseStatusCodeKey].ShouldEqual(HttpStatusCode.UseProxy.As<int>());
        }

        [Test]
        public void should_set_response_code_and_description()
        {
            const string description = "why u no make good request?";
            response.WriteResponseCode(HttpStatusCode.BadRequest, description);
            environment[OwinConstants.ResponseStatusCodeKey].ShouldEqual(HttpStatusCode.BadRequest.As<int>());
            environment[OwinConstants.ResponseReasonPhraseKey].ShouldEqual(description);
        }
    }
}