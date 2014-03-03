using AspNetApplication;
using FubuMVC.Core.Runtime;
using FubuMVC.IntegrationTesting;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class writing_a_file_to_output
    {
        [Test]
        public void can_write_the_contents_of_a_file_to_the_output()
        {
            var response = TestApplication.Endpoints.GetByInput(new FileInput
            {
                Name = "Test.txt"
            });

            response.ContentTypeShouldBe(MimeType.Text);
            response.ReadAsText().ShouldContain("Some text here");
        }
    }
}