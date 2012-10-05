using ViewEngineIntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.HelloPartial;
using ViewEngineIntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesNativePartials;
using ViewEngineIntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesPartial;
using ViewEngineIntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesTransferTo;
using FubuTestingSupport;
using NUnit.Framework;

namespace ViewEngineIntegrationTesting.ViewEngines.Spark.PartialNoLayout
{
    [TestFixture]
    public class PartialNoLayoutIntegrationTester : SharedHarnessContext
    {
        [Test]
        public void does_not_apply_layout_when_invoked_as_partial()
        {
            var text = endpoints.Get<UsesPartialEndpoint>(x => x.Execute()).ReadAsText();

            text.ShouldContain("<h1>Uses partial</h1>");
            text.ShouldContain("<h1>Default layout</h1>");
            text.ShouldContain("<p>In a partial</p>");
            text.ShouldNotContain("<h1>This layout means FAIL!</h1>");
        }

        [Test]
        public void invoking_action_normally_should_render_the_correct_layout()
        {
            var text = endpoints.Get<HelloPartialEndpoint>(x => x.Render()).ReadAsText();
            text.ShouldContain("<p>In a partial</p>");
            text.ShouldContain("<h1>This layout means FAIL!</h1>");
        }

        [Test]
        public void native_partials_happy_path()
        {
            var text = endpoints.Get<UsesNativeEndpoint>(x => x.Render()).ReadAsText();

            text.ShouldContain("<div>Hello Native</div>");
        }

        [Test]
        public void partials_should_still_have_access_to_master_layout_content_areas()
        {
            endpoints.Get<UsesPartialEndpoint>(x => x.Execute()).ScriptNames()
                .ShouldContain("_/herp/derp.js");
        }

        [Test]
        public void should_apply_layout_when_transfered_to()
        {
            var text = endpoints.Get<TransferToEndpoint>(x => x.Tranfer()).ReadAsText();
            text.ShouldContain("<p>In a partial</p>");
            text.ShouldContain("<h1>This layout means FAIL!</h1>");
        }

        [Test]
        public void should_redirect_to()
        {
            var text = endpoints.Get<RedirectToEndpoint>(x => x.Redirect()).ReadAsText();

            text.ShouldContain("<h1>Uses partial</h1>");
            text.ShouldContain("<h1>Default layout</h1>");
            text.ShouldContain("<p>In a partial</p>");
            text.ShouldNotContain("<h1>This layout means FAIL!</h1>");
        }
    }
}