using System;
using System.Net;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;
using FubuCore;

namespace FubuMVC.Core.Assets.IntegrationTesting
{
    [TestFixture]
    public class Image_FubuPage_Extension_methods_Integrated_tester : FubuPageExtensionContext
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ConventionEndpoint>();
        }

        [Test]
        public void ImageFor()
        {
            execute(page => page.ImageFor("something.png"));
            theResult.ShouldEndWith("_content/images/something.png\" />");
        }

        [Test]
        public void ImageUrl()
        {
            execute(page => page.ImageUrl("something.png"));

            theResult.ShouldEqual("{0}/_content/images/something.png".ToFormat(BaseAddress));
        }
    }

    [TestFixture]
    public class FubuPageExtensionContext : FubuRegistryHarness
    {
        protected string theResult;

        protected override void beforeRunning()
        {
            theResult = string.Empty;
        }

        protected void execute(Func<IFubuPage<ImageTarget>, object> func)
        {
            ConventionEndpoint.Source = func;

            var response = endpoints.Get<ConventionEndpoint>(x => x.get_result());
            response.StatusCodeShouldBe(HttpStatusCode.OK);

            theResult = response.ReadAsText();
        }
    }

    public class ConventionEndpoint
    {
        private readonly FubuHtmlDocument<ImageTarget> _document;
        public static Func<IFubuPage<ImageTarget>, object> Source = page => "nothing";

        public ConventionEndpoint(FubuHtmlDocument<ImageTarget> document)
        {
            _document = document;
        }

        public string get_result()
        {
            return Source(_document).ToString();
        }
    }

    public class ImageTarget{}
}