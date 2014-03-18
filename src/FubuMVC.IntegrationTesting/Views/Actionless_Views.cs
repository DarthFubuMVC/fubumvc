using System.Security.Cryptography.X509Certificates;
using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views
{
    [TestFixture]
    public class Actionless_Views : ViewIntegrationContext
    {
        public Actionless_Views()
        {
            SparkView<ActionlessView1>("View1").WriteLine("I'm in view 1!");
            RazorView<ActionlessView2>("View2").WriteLine("I'm in view 2!");
            RazorView<ActionlessView3>("View2").WriteLine("I'm in view 3!");

            RazorView<ActionlessViewWithPartials>("HasPartials").Write(@"

I am in the view w/ partials

@this.Partial(new FubuMVC.IntegrationTesting.Views.ActionlessView3())

");
        }


        [Test]
        public void actionless_view_is_a_full_route_with_the_url_pattern_in_Spark()
        {
            Scenario.Get.Input<ActionlessView1>();

            Scenario.ContentShouldContain("I'm in view 1!");
        }

        [Test]
        public void actionless_view_is_a_full_route_with_the_url_pattern_in_Razor()
        {
            Scenario.Get.Input<ActionlessView2>();

            Scenario.ContentShouldContain("I'm in view 2!");
        }

        [Test]
        public void can_use_actionless_views_as_partials_if_they_have_no_url_pattern()
        {
            Scenario.Get.Input<ActionlessViewWithPartials>();

            Scenario.ContentShouldContain("I am in the view w/ partials");
            Scenario.ContentShouldContain("I'm in view 3!");
        }
    }

    [UrlPattern("has_some_partials")]
    public class ActionlessViewWithPartials
    {
        
    }

    [UrlPattern("actionless1")]
    public class ActionlessView1
    {
        
    }

    [UrlPattern("actionless2")]
    public class ActionlessView2
    {

    }

    public class ActionlessView3{}
}