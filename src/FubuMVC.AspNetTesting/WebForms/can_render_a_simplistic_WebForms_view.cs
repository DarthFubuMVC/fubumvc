using AspNetApplication.WebForms;
using FubuMVC.Core.Runtime;
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
            TestApplication.DebugRemoteBehaviorGraph();

            TestApplication.Endpoints.GetByInput(new WebFormInput{Name = "Jeremy"})
                .ContentTypeShouldBe(MimeType.Html)
                .ReadAsText().ShouldContain("My name is Jeremy");
        }
    }
}