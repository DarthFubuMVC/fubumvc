using FubuMVC.Core.Assets;
using FubuMVC.Core.Registration;
using FubuMVC.Razor;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class modifies_the_WatchSettings
    {
        [Test]
        public void adds_spark_and_bindings_xml_file()
        {
            var graph = BehaviorGraph.BuildFrom(x => {
                x.Import<RazorViewFacility>();
            });

            var settings = graph.Settings.Get<AssetSettings>();
        
            settings.ContentMatches.ShouldContain("*.cshtml");
        }
    }
}