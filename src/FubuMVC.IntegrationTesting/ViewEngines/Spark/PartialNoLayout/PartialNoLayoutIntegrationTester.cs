using System;
using FubuMVC.Core;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.IntegrationTesting.ViewEngines.Spark.HelloSpark;
using FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.HelloPartial;
using FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesPartial;
using FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesTransferTo;
using FubuMVC.Spark;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout
{
    [TestFixture]
    public class PartialNoLayoutIntegrationTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Import<SparkEngine>();
            registry.Actions.IncludeType<UsesPartialController>();
            registry.Actions.IncludeType<HelloPartialController>();
            registry.Actions.IncludeType<TransferToController>();
            registry.Actions.IncludeType<RedirectToController>();
            registry.Views.TryToAttachWithDefaultConventions();
        }

        [Test]
        public void does_not_apply_layout_when_invoked_as_partial()
        {
            var text = endpoints.Get<UsesPartialController>(x => x.Execute()).ReadAsText();

            text.ShouldContain("<h1>Uses partial</h1>");
            text.ShouldContain("<h1>Default layout</h1>");
            text.ShouldContain("<p>In a partial</p>");
            text.ShouldNotContain("<h1>This layout means FAIL!</h1>");
        }

        [Test]
        public void should_apply_layout_when_transfered_to()
        {
            var text = endpoints.Get<TransferToController>(x => x.Tranfer()).ReadAsText();
            text.ShouldContain("<p>In a partial</p>");
            text.ShouldContain("<h1>This layout means FAIL!</h1>");
        }

        [Test]
        public void should_redirect_to()
        {
            var text = endpoints.Get<RedirectToController>(x => x.Redirect()).ReadAsText();
            
            text.ShouldContain("<h1>Uses partial</h1>");
            text.ShouldContain("<h1>Default layout</h1>");
            text.ShouldContain("<p>In a partial</p>");
            text.ShouldNotContain("<h1>This layout means FAIL!</h1>");
        }
    }
}