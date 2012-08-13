using AspNetApplication.FileUpload;
using AspNetApplication.WebForms;
using FubuMVC.Core.Runtime;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuMVC.IntegrationTesting.Conneg;
using FubuTestingSupport;

namespace FubuMVC.AspNetTesting.WebForms
{
    [TestFixture]
    public class can_render_a_simplistic_WebForms_view
    {
        [Test]
        public void get_a_simple_view_with_no_master_page_or_partials()
        {
            // Brandon, check this out as a helper.
            //TestApplication.DebugRemoteBehaviorGraph();

            TestApplication.Endpoints.GetByInput(new WebFormInput{Name = "Jeremy"})
                .ContentTypeShouldBe(MimeType.Html)
                .ReadAsText().ShouldContain("My name is Jeremy");
        }

        [Test]
        public void post_a_simple_form()
        {
            TestApplication.Endpoints.PostAsForm(new WebFormInput {Name = "Chad"})
                .ContentTypeShouldBe(MimeType.Html)
                .ReadAsText().ShouldContain("My name is Chad");
        }

        [Test]
        public void model_binding_to_non_string_aspnet_objects()
        {
            var response = TestApplication.Endpoints.GetByInput(new AspNetModelBindingInput())
                .ContentTypeShouldBe(MimeType.Html)
                .ReadAsText();
            
            response.ShouldContain("Browser: Unknown");
            response.ShouldContain("ContentLength: 0");
            response.ShouldContain("Cookies: 0");
            response.ShouldContain("HttpRequest.UserAgent: EndpointDriver User Agent 1.0");
            response.ShouldContain("HttpRequest[\"User-Agent\"]: EndpointDriver User Agent 1.0");
            
        }
    }
}