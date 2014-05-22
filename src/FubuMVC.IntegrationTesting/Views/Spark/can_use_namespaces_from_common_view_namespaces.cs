using System.Xml.Serialization;
using FubuMVC.Core;
using FubuMVC.Core.View;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class can_use_namespaces_from_common_view_namespaces : ViewIntegrationContext
    {
        public can_use_namespaces_from_common_view_namespaces()
        {
            SparkView<FakeFoo>("foo").Write(@"
!{this.SomeFakeFoo()}

");
        }

        [Test]
        public void happily_uses_the_namespaces_imported_automatically()
        {
            Scenario.Get.Input<FakeFoo>();
            Scenario.ContentShouldContain("some fake foo stuff");
        }
    }

    public static class FakeFubuPageExtensions
    {
        public static string SomeFakeFoo(this IFubuPage page)
        {
            return "some fake foo stuff";
        }
    }

    [UrlPattern("fake/foo")]
    public class FakeFoo
    {
        
    }
}