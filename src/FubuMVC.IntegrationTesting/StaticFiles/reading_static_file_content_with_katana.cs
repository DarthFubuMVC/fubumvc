using System.Net;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Files;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.IntegrationTesting.StaticFiles
{
    [TestFixture]
    public class reading_static_file_content_with_katana
    {
        private readonly IFubuFile file = FubuFile.Load("Sample.js");

        [Test]
        public void read_file_with_hit_on_etag()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("Sample.js").Etag(file.Etag());
                _.StatusCodeShouldBe(HttpStatusCode.NotModified);
            });
        }

        [Test]
        public void can_return_the_HEAD_for_a_file()
        {
            TestHost.Scenario(_ =>
            {
                _.Head.Url("Sample.js");

                _.Header(HttpResponseHeaders.ETag).SingleValueShouldEqual("\"" + file.Etag() + "\"");

                _.Response.LastModified().ShouldBe(file.LastModified());
            });
        }

        [Test]
        public void can_return_the_text_of_a_txt_file()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("Sample.js");
                _.StatusCodeShouldBeOk();
                _.ContentShouldContain("This is some sample data in a static file");
            });
        }

        [Test]
        public void can_return_the_text_of_a_txt_file_on_etag_miss()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("Sample.js").Etag(file.Etag() + "!!!");
                _.StatusCodeShouldBeOk();
                _.ContentShouldContain("This is some sample data in a static file");
            });
        }

        [Test]
        public void get_304_on_if_modified_since_pass()
        {
            var lastModified = file.LastModified().ToUniversalTime().AddMinutes(10);

            TestHost.Scenario(_ =>
            {
                _.Get.Url("Sample.js");
                _.Request.AppendHeader(HttpRequestHeaders.IfModifiedSince, lastModified.ToString("R"));

                _.StatusCodeShouldBe(HttpStatusCode.NotModified);
            });
        }

        [Test]
        public void get_the_file_on_if_modified_since_and_has_been_modified()
        {
            var ifModifiedSince = file.LastModified().ToUniversalTime().AddMinutes(-20);

            TestHost.Scenario(_ =>
            {
                _.Get.Url("Sample.js");
                _.Request.AppendHeader(HttpRequestHeaders.IfModifiedSince, ifModifiedSince.ToString("R"));

                _.StatusCodeShouldBeOk();
                _.ContentShouldContain("This is some sample data in a static file");
            });
        }
    }
}