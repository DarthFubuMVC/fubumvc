using System.Threading;
using FubuMVC.IntegrationTesting.Views.Razor;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class No_layout_with_use_none : ViewIntegrationContext
    {
        public No_layout_with_use_none()
        {
            SparkView<NoLayoutModel>("View1").Write(@"
<use master='none'/>

<h1>Some stuff</h1>

");

            SparkView("Shared/Application").WriteLine("Text from the Application layout");
        }

        [Test, Explicit("Too flakey w/ the file system")]
        public void should_not_use_any_layout_if_layout_equals_none()
        {
            Scenario.Get.Action<NoLayoutEndpoint>(x => x.get_no_layout());

            Scenario.ContentShouldContain("<h1>Some stuff</h1>");
            Scenario.ContentShouldNotContain("Text from the Application layout");
        }
    }
}