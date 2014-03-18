using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class Render_views_from_bottles : ViewIntegrationContext
    {
        public Render_views_from_bottles()
        {

            SparkView("Shared/Application").Write(@"
<use content='view'/>
I am in the main application layout");

            InBottle("BottleA");
            SparkView<BottleModel>("Bottle").Write(@"

<p>I am from a bottle!</p>
<SayHello />

");

            File("Shared/bindings.xml").Write(@"

<bindings>
  <element name='SayHello'>'Hello'</element>
</bindings>

");


        }

        [Test]
        public void can_happily_render_a_bottle_view_and_it_has_the_application_template_from_the_app()
        {
            Scenario.Get.Action<FromBottleEndpoint>(x => x.get_bottle_view());

            Scenario.ContentShouldContain("I am in the main application layout");
            Scenario.ContentShouldContain("<p>I am from a bottle!</p>");
        }

        [Test]
        public void can_use_bindings_from_a_bottle_automatically()
        {
            Scenario.Get.Action<FromBottleEndpoint>(x => x.get_bottle_view());

            Scenario.ContentShouldContain("Hello");
        }
    }

    public class BottleModel
    {
        
    }

    public class FromBottleEndpoint
    {
        public BottleModel get_bottle_view()
        {
            return new BottleModel();
        }
    }
}