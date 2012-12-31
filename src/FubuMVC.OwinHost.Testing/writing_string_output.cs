using System.Net;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class writing_string_output
    {
        [Test]
        public void can_write_strings_to_the_output()
        {
            Harness.Endpoints.Get<StringEndpoint>(x => x.get_hello()).ContentShouldBe(MimeType.Text, "Hello.")
                .StatusCode.ShouldEqual(HttpStatusCode.OK);
        }
    }

    public class StringEndpoint
    {
        public string get_hello()
        {
            return "Hello.";
        }
    }
}