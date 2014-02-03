using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.StaticFiles
{
    [TestFixture]
    public class reading_static_file_content_with_katana
    {
        private readonly IFubuFile file = FubuFile.Load("Sample.js");

        [Test]
        public void read_file_with_hit_on_etag()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(autoFindPort: true))
            {
                server.Endpoints.Get("Sample.js", etag:file.Etag())
                    .StatusCodeShouldBe(HttpStatusCode.NotModified);

            }
        }

        [Test]
        public void can_return_the_HEAD_for_a_file()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(autoFindPort: true))
            {
                server.Endpoints.Head("Sample.js")
                    .StatusCodeShouldBe(HttpStatusCode.OK)
                    .LastModifiedShouldBe(file.LastModified())
                    .EtagShouldBe(file.Etag());

            }
        }

        [Test]
        public void can_return_the_text_of_a_txt_file()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(autoFindPort: true))
            {
                var response = server.Endpoints.Get("Sample.js");
                response.StatusCodeShouldBe(HttpStatusCode.OK);
                response.ReadAsText().ShouldContain("This is some sample data in a static file");
                
            }
        }

        [Test]
        public void can_return_the_text_of_a_txt_file_on_etag_miss()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(autoFindPort: true))
            {
                var response = server.Endpoints.Get("Sample.js", etag:file.Etag() + "!!!");
                response.StatusCodeShouldBe(HttpStatusCode.OK);
                response.ReadAsText().ShouldContain("This is some sample data in a static file");

            }
        }

        [Test]
        public void get_304_on_if_modified_since_pass()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(autoFindPort: true))
            {
                server.Endpoints.Get("Sample.js", ifModifiedSince: file.LastModified().ToUniversalTime().AddMinutes(10))
                    .StatusCodeShouldBe(HttpStatusCode.NotModified);

            }
        }

        [Test]
        public void get_the_file_on_if_modified_since_and_has_been_modified()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(autoFindPort: true))
            {
                server.Endpoints.Get("Sample.js", ifModifiedSince: file.LastModified().ToUniversalTime().AddMinutes(-20))
                    .StatusCodeShouldBe(HttpStatusCode.OK)
                    .ReadAsText().ShouldContain("This is some sample data in a static file");

            }
        }
    }
}