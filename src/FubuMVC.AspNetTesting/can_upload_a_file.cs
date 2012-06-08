using AspNetApplication.FileUpload;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class can_upload_a_file
    {
        [Test]
        public void upload_a_file_using_input_type_file_with_form_multipart()
        {
            TestApplication.Endpoints.PostFile(new FileUploadInput(), "File1").ReadAsText().ShouldContain("success");
        }
    }
}