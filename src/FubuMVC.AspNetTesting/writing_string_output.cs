using System.Net;
using AspNetApplication;
using FubuMVC.Core.Runtime;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;

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