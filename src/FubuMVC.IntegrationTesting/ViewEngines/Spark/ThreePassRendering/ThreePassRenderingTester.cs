using System.Text;
using FubuMVC.Core;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.ThreePassRendering
{
    public class ThreePassRenderingTester : FubuRegistryHarness
    {
        private string theResult;
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ThreePassController>();
            registry.Views.TryToAttachWithDefaultConventions();

            theResult = new StringBuilder()
                .AppendLine("<html>")
                .AppendLine("<head>")
                .AppendLine(@"<script src=""/content/js/jquery-ui-accordian.js""/></script>")
                .AppendLine("</head>")
                .AppendLine("<body>")
                .AppendLine("<h1>Three Pass Rendering Test!</h1>")
                .AppendLine(@"<div id=""sidebar"">Say hello: Hello</div>")
                .AppendLine("</body>")
                .Append("</html>").Replace("\r", string.Empty)
                .ToString();
        }

        [Test]
        public void three_pass_renders_correctly()
        {
            endpoints.Get<ThreePassController>(x => x.ThreePassSample(null))
                .ReadAsText().ShouldEqual(theResult);
        }
    }
}