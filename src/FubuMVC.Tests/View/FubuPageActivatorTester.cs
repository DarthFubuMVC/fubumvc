using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class FubuPageActivatorTester : InteractionContext<FubuPageActivator<TestModelForActivation>>
    {
        private InMemoryFubuRequest theRequest;
        private IFubuPage<TestModelForActivation> thePage;

        protected override void beforeEach()
        {
            theRequest = new InMemoryFubuRequest();
            Services.Inject<IFubuRequest>(theRequest);

            MockFor<IServiceLocator>().Stub(x => x.GetInstance<IFubuRequest>()).Return(theRequest);

            thePage = MockFor<IFubuPage<TestModelForActivation>>();
        }

        [Test]
        public void use_the_explicit_type_requested_first_if_it_exists()
        {
            var model = new TestModelForActivation();
            theRequest.Set(model);

            ClassUnderTest.Activate(thePage);

            thePage.AssertWasCalled(x => x.Model = model);
        }

        [Test]
        public void use_the_first_value_of_a_sub_type_if_one_exists()
        {
            var model = new SubclassTestModelForActivation();
            theRequest.Set(model);

            ClassUnderTest.Activate(thePage);

            thePage.AssertWasCalled(x => x.Model = model);
        }
    }

    [TestFixture]
    public class when_the_view_model_type_has_not_been_filled_before_the_view_is_executed : InteractionContext<FubuPageActivator<TestModelForActivation>>
    {
        private TestModelForActivation theModel;
        private IFubuPage<TestModelForActivation> thePage;

        protected override void beforeEach()
        {
            theModel = new TestModelForActivation();
            MockFor<IFubuRequest>().Stub(x => x.Has<TestModelForActivation>()).Return(false);
            MockFor<IFubuRequest>().Stub(x => x.Find<TestModelForActivation>()).Return(new TestModelForActivation[0]);
            MockFor<IFubuRequest>().Stub(x => x.Get<TestModelForActivation>()).Return(theModel);

            thePage = MockFor<IFubuPage<TestModelForActivation>>();

            MockFor<IServiceLocator>().Stub(x => x.GetInstance<IFubuRequest>()).Return(MockFor<IFubuRequest>());

            ClassUnderTest.Activate(thePage);
        }

        [Test]
        public void is_able_to_set_the_model_on_the_page_by_calling_through_to_FubuRequest_which_would_in_turn_trigger_model_binding()
        {
            thePage.AssertWasCalled(x => x.Model = theModel);
        }
    }
    

    public class TestModelForActivation : SimpleFubuPage<TestModelForActivation>
    {
    }

    public class SubclassTestModelForActivation : TestModelForActivation
    {
    }
}