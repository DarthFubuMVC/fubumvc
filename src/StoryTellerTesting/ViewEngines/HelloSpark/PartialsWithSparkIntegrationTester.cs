using System;
using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using IntegrationTesting.Conneg;
using NUnit.Framework;
using FubuTestingSupport;
using FubuMVC.Spark;

namespace IntegrationTesting.ViewEngines.HelloSpark
{
    [TestFixture]
    public class Partials_and_actionless_views_WithSparkIntegrationTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.UseSpark();
            registry.IncludeDiagnostics(true);
            registry.Actions.IncludeType<PartialController>();
            registry.Views.RegisterActionLessViews(v => v.ViewModel == typeof (PartialInput));
            registry.Views.RegisterActionLessViews(v => v.ViewModel == typeof (MoreInput));
            registry.Views.TryToAttachWithDefaultConventions();
        }

        [Test]
        public void pair_of_nested_partials()
        {
            Debug.WriteLine(endpoints.Get<BehaviorGraphWriter>(x => x.PrintRoutes()));

            var text = endpoints.Get<PartialController>(x => x.get_partials()).ReadAsText();

            text.ShouldContain("<h1>My name is Shiner</h1>");
            text.ShouldContain("<p>I am a 7 year old Labrador mix</p>");
        }
    }

    public class PartialController
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