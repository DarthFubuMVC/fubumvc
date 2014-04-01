using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Assets
{
    [TestFixture]
    public class Tag_building_with_required_scripts : AssetIntegrationContext
    {
        public Tag_building_with_required_scripts()
        {
            File("script1.js");
            File("script2.js");
            File("script3.js");
            File("script4.js");
            File("script5.js");
            File("script6.js");
        }

        [Test]
        public void should_write_the_scripts_that_are_queued()
        {
            var builder = TagBuilder();

            builder.RequireScript("script1.js", "script2.js");

            builder.BuildScriptTags(new []{"script3.js"}).Select(x => x.Attr("src"))
                .ShouldHaveTheSameElementsAs("/script1.js", "/script2.js", "/script3.js");
        }

        [Test]
        public void should_not_write_the_same_script_twice()
        {
            var builder = TagBuilder();

            builder.RequireScript("script1.js", "script2.js");

            builder.BuildScriptTags(new[] { "script3.js" }).Select(x => x.Attr("src"))
                .ShouldHaveTheSameElementsAs("/script1.js", "/script2.js", "/script3.js");

            builder.RequireScript("script3.js");
            builder.BuildScriptTags(new string[]{"script1.js", "script3.js"}).Any()
                .ShouldBeFalse();

            builder.BuildScriptTags(new string[] { "script1.js", "script2.js" }).Any()
                .ShouldBeFalse();

            builder.RequireScript("script1.js");

            builder.BuildScriptTags(new string[] { "script1.js" }).Any()
    .ShouldBeFalse();
        }
    
    }
}