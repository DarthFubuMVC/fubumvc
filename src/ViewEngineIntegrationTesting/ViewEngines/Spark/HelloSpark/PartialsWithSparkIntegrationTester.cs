using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace ViewEngineIntegrationTesting.ViewEngines.Spark.HelloSpark
{
    [TestFixture]
    public class Partials_and_actionless_views_WithSparkIntegrationTester : SharedHarnessContext
    {
        [Test]
        public void pair_of_nested_partials()
        {
            var text = endpoints.Get<PartialSparkEndpoints>(x => x.get_partials()).ReadAsText();

            text.ShouldContain("<h1>My name is Shiner</h1>");
            text.ShouldContain("<p>I am a 7 year old Labrador mix</p>");
        }
    }

    public class PartialSparkEndpoints
    {
        public FullViewModel get_partials()
        {
            return new FullViewModel{
                PartialModel = new PartialInput{
                    Name = "Shiner",
                    NestedInput = new MoreInput{
                        Description = "I am a 7 year old Labrador mix"
                    }
                }
            };
        }
    }

    public class PartialInput
    {
        public string Name { get; set; }
        public MoreInput NestedInput { get; set; }
    }

    public class MoreInput
    {
        public string Description { get; set; }
    }

    public class FullViewModel
    {
        public PartialInput PartialModel { get; set; }
    }
}