using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.View.Attachment;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace ViewEngineIntegrationTesting.ViewAttachment
{
    [TestFixture]
    public class ProfiledAttachmentTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ProfileController>();

            registry.AlterSettings<ViewAttachmentPolicy>(x => { x.Profile<Mobile>("m"); });
        }

        [Test]
        public void fetching_the_resource_in_a_way_that_trips_off_the_special_profile_should_give_you_the_mobile_view()
        {
            endpoints.GetByInput(new ProfileInput
            {
                Name = "mobile"
            }).ReadAsText().ShouldContain("<p>I am the mobile view</p>");
        }

        [Test]
        public void fetching_the_resource_when_it_does_not_match_the_special_profile()
        {
            endpoints.GetByInput(new ProfileInput
            {
                Name = "wrong"
            }).ReadAsText().ShouldContain("<p>I am the regular view</p>");
        }
    }


    [TestFixture]
    public class ProfiledAttachment_with_inferred_view_attachment_filters_Tester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ProfileController>();

            // I want the default to work here.
            //registry.Views.TryToAttachWithDefaultConventions();
            registry.AlterSettings<ViewAttachmentPolicy>(x => x.Profile<Mobile>("m."));
        }

        [Test]
        public void fetching_the_resource_in_a_way_that_trips_off_the_special_profile_should_give_you_the_mobile_view()
        {
            endpoints.GetByInput(new ProfileInput
            {
                Name = "mobile"
            }).ReadAsText().ShouldContain("<p>I am the mobile view</p>");
        }

        [Test]
        public void fetching_the_resource_when_it_does_not_match_the_special_profile()
        {
            endpoints.GetByInput(new ProfileInput
            {
                Name = "wrong"
            }).ReadAsText().ShouldContain("<p>I am the regular view</p>");
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

        #region IConditional Members

        public bool ShouldExecute()
        {
            return _request.Get<ProfileInput>().Name == "mobile";
        }

        #endregion
    }

    public class ProfileController
    {
        public ProfileOutput get_profiled_view_Name(ProfileInput input)
        {
            return new ProfileOutput();
        }
    }
}