using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
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
            RazorView<ActionlessView3>("View3").WriteLine("I'm in view 3!");

            RazorView<ActionlessViewWithPartials>("HasPartials").Write(@"

I am in the view w/ partials

@this.Partial(new FubuMVC.IntegrationTesting.Views.ActionlessView3())

");
        }

        [Test]
        public void action_less_view_has_UrlCategory_for_VIEW()
        {
            BehaviorGraph.Behaviors.Single(x => typeof (ActionlessView1) == x.InputType())
                .Category.ShouldEqual(Categories.VIEW);
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
        public void can_resolve_actionless_views_from_partial_invoker_by_category()
        {
            var chain = Services.GetInstance<IChainResolver>().Find(new ChainSearch
            {
                CategoryMode = CategorySearchMode.Relaxed,
                CategoryOrHttpMethod = Categories.VIEW,
                Type = typeof(ActionlessView3),
                TypeMode = TypeSearchMode.Any
            });

            chain.ShouldNotBeNull();
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


    public class ActionlessViewAjaxEndpoint
    {
        public AjaxContinuation get_alternative_for_actionless_view(ActionlessView3 input)
        {
            return AjaxContinuation.Successful();
        }
    }
}