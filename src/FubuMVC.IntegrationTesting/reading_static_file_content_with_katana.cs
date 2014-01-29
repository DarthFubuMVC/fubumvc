using System.Net;
using FubuMVC.Core;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture, Ignore("not ready yet")]
    public class reading_static_file_content_with_katana
    {
        [Test]
        public void can_return_the_text_of_a_txt_file()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded(autoFindPort: true))
            {
                var response = server.Endpoints.Get("Sample.txt");
                response.ReadAsText().ShouldContain("This is some sample data in a static file");
                response.StatusCodeShouldBe(HttpStatusCode.OK);
            }
        }
    }
}