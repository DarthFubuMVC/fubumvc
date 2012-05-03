using System;
using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using IntegrationTesting.Conneg;
using NUnit.Framework;
using FubuMVC.Spark;
using FubuTestingSupport;

namespace IntegrationTesting.ViewAttachment
{
    [TestFixture]
    public class ProfiledAttachmentTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.UseSpark();
            registry.Actions.IncludeType<ProfileController>();
            registry.Views.TryToAttachWithDefaultConventions();
            registry.Views.Profile<Mobile>("m.");
            registry.IncludeDiagnostics(true);
        }

        [Test]
        public void what_is_there()
        {
            var text = endpoints.Get<BehaviorGraphWriter>(x => x.PrintRoutes()).ReadAsText();

            Debug.WriteLine(text);
        }

        [Test]
        public void fetching_the_resource_when_it_does_not_match_the_special_profile()
        {
            endpoints.GetByInput(new ProfileInput { Name = "wrong" }).ReadAsText().ShouldContain("<p>I am the regular view</p>");
        }

        [Test]
        public void fetching_the_resource_in_a_way_that_trips_off_the_special_profile_should_give_you_the_mobile_view()
        {
            endpoints.GetByInput(new ProfileInput { Name = "mobile" }).ReadAsText().ShouldContain("<p>I am the mobile view</p>");
        }
    }

    public class ProfileInput
    {
        public string Name { get; set; }
    }

    public class ProfileOutput
    {
        
    }

    public class Mobile : IConditional
    {
        private readonly IFubuRequest _request;

        public Mobile(IFubuRequest request)
        {
            _request = request;
        }

        public bool ShouldExecute()
        {
            return _request.Get<ProfileInput>().Name == "mobile";
        }
    }

    public class ProfileController
    {
        public ProfileOutput get_profiled_view_Name(ProfileInput input)
        {
            return new ProfileOutput();
        }
    }
}