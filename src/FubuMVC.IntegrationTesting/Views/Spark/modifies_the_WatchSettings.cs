using FubuMVC.Core.Assets;
using FubuMVC.Core.Registration;
using FubuMVC.Spark;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class modifies_the_WatchSettings
    {
        [Test]
        public void adds_spark_and_bindings_xml_file()
        {
            var graph = BehaviorGraph.BuildFrom(x => {
                x.Import<SparkViewFacility>();
            });

            var settings = graph.Settings.Get<AssetSettings>();
        
            settings.ContentMatches.ShouldContain("*.spark");
            settings.ContentMatches.ShouldContain("bindings.xml");
        }
    }
}