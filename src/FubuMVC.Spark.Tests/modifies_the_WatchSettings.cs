using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests
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