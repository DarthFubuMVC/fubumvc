using System.Net;
using AspNetApplication;
using FubuMVC.Core.Runtime;
using FubuMVC.IntegrationTesting;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class writing_string_output
    {
        [Test]
        public void can_write_strings_to_the_output()
        {
            TestApplication.Endpoints.Get<StringEndpoint>(x => x.get_hello())
                .ContentShouldBe(MimeType.Text, "Hello.")
                .StatusCode.ShouldEqual(HttpStatusCode.OK);
        }
    }
}