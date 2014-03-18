using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class Partials : ViewIntegrationContext
    {
        public Partials()
        {
            SparkView<FullViewModel>("Full").Write(@"
<p>The partial is below</p>

!{this.Partial(Model.PartialModel)}

<p>The partial is above</p>
");

            SparkView<PartialInput>("Partial1").Write(@"
<h1>My name is !{Model.Name}</h1>
!{this.Partial(Model.NestedInput)}
");

            SparkView<MoreInput>("Partial2").Write(@"
<p>!{Model.Description}</p>

");

        }

        [Test]
        public void pair_of_nested_partials()
        {
            Scenario.Get.Action<PartialSparkEndpoints>(x => x.get_partials());

            Scenario.ContentShouldContain("<h1>My name is Shiner</h1>");
            Scenario.ContentShouldContain("<p>I am a 7 year old Labrador mix</p>");
        }
    }

    public class PartialSparkEndpoints
    {
        public FullViewModel get_partials()
        {
            return new FullViewModel
            {
                PartialModel = new PartialInput
                {
                    Name = "Shiner",
                    NestedInput = new MoreInput
                    {
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