using System;
using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Razor;
using FubuMVC.WebForms;
using IntegrationTesting.Conneg;
using NUnit.Framework;
using FubuTestingSupport;

namespace IntegrationTesting.ViewEngines.HelloRazor
{
    [TestFixture]
    public class RazorEngineIntegrationTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<HelloRazorController>();

            registry.IncludeDiagnostics(true);

            //registry.UseSpark();
            registry.UseRazor();

            registry.Views
                .TryToAttachWithDefaultConventions();
        }

        [Test]
        public void simple_get_of_razor_view_with_no_template()
        {
            var text = endpoints.Get<HelloRazorController>(x => x.SayHello(new HelloWorldRazorInputModel()))
                .ReadAsText();

            text.ShouldContain("Hello World! FubuMVC + Razor");

            // Partial text
            text.ShouldContain("<p>I'm from a partial</p>");
        }

    }
}