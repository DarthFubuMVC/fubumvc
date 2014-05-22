using FubuMVC.Core.View;
using FubuMVC.IntegrationTesting.Views.Spark;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class can_use_the_auto_imported_namespaces : ViewIntegrationContext
    {
        public can_use_the_auto_imported_namespaces()
        {
            RazorView<FakeFoo>("foo").Write(@"

@this.SomeFakeFoo()

");
        }

        [Test]
        public void can_use_an_extension_in_a_namespace_matching_a_view_model()
        {
            Scenario.Get.Input<FakeFoo>();
            Scenario.ContentShouldContain("some fake foo stuff");
        }
    }


}