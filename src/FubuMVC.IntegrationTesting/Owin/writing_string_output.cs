using System.Net;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class writing_string_output
    {
        [Test]
        public void can_write_strings_to_the_output()
        {
            HarnessApplication.Run(endpoints => {
                endpoints.Get<StringEndpoint>(x => x.get_hello()).ContentShouldBe(MimeType.Text, "Hello.")
                    .StatusCode.ShouldEqual(HttpStatusCode.OK);
            });
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