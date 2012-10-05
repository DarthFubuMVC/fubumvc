using FubuMVC.Core;
using ViewEngineIntegrationTesting.ViewEngines.Razor.MultipleLayouts.Features.One.UsesClosest;
using ViewEngineIntegrationTesting.ViewEngines.Razor.MultipleLayouts.Features.Two.UsesDefault;
using FubuMVC.Razor;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;

namespace ViewEngineIntegrationTesting.ViewEngines.Razor.MultipleLayouts
{
    [TestFixture]
    public class RazorEngineMultipleLayoutsIntegrationTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ClosestController>();
            registry.Actions.IncludeType<UsesDefaultController>();
        }

        [Test]
        public void uses_layout_in_closest_shared_directory_when_found()
        {
            var text = endpoints.Get<ClosestController>(x => x.Execute())
                .ReadAsText();

            text.ShouldContain("<h1>Closest.cshtml</h1>");

            // Layout text
            text.ShouldContain("<h2>One/Shared/Application.cshtml</h2>");
            // Recursive Layout text
            text.ShouldContain("<h2>Default Html.cshtml</h2>");
        }

        [Test]
        public void uses_default_layout_when_none_are_found_in_closer_directory()
        {
            var text = endpoints.Get<UsesDefaultController>(x => x.Execute())
                .ReadAsText();

            text.ShouldContain("<h1>UsesDefault.cshtml</h1>");

            // Layout text
            text.ShouldContain("<h2>Default Layout</h2>");
        }
    }
}