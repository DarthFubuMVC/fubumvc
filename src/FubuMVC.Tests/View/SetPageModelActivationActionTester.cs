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
    public class when_the_type_requested_is_in_the_request :
        InteractionContext<SetPageModelActivationAction<TestModelForActivation>>
    {
        private IServiceLocator _theServices;
        private InMemoryFubuRequest _theRequest;
        private SimpleFubuPage<TestModelForActivation> _simpleFubuPage;

        protected override void beforeEach()
        {
            _theServices = MockRepository.GenerateMock<IServiceLocator>();

            _theRequest = new InMemoryFubuRequest();
            _theRequest.Set(new TestModelForActivation());
            _theRequest.Set(new SubclassTestModelForActivation());
            _theServices.Stub(x => x.GetInstance<IFubuRequest>()).Return(_theRequest);
            _simpleFubuPage = new SimpleFubuPage<TestModelForActivation>();
            ClassUnderTest.Activate(_theServices, _simpleFubuPage);
        }

        [Test]
        public void should_set_the_page_model_to_the_exact_type_if_its_in_the_request()
        {
            _simpleFubuPage.Model.ShouldBeOfType<TestModelForActivation>();
        }
    }

    [TestFixture]
    public class when_a_subclass_of_the_type_requested_is_in_the_request :
        InteractionContext<SetPageModelActivationAction<TestModelForActivation>>
    {
        private IServiceLocator _theServices;
        private InMemoryFubuRequest _theRequest;
        private SimpleFubuPage<TestModelForActivation> _simpleFubuPage;

        protected override void beforeEach()
        {
            _theServices = MockRepository.GenerateMock<IServiceLocator>();

            _theRequest = new InMemoryFubuRequest();
            _theRequest.Set(new SubclassTestModelForActivation());
            _theServices.Stub(x => x.GetInstance<IFubuRequest>()).Return(_theRequest);
            _simpleFubuPage = new SimpleFubuPage<TestModelForActivation>();
            ClassUnderTest.Activate(_theServices, _simpleFubuPage);
        }

        [Test]
        public void should_set_the_page_model_to_the_exact_type_if_its_in_the_request()
        {
            _simpleFubuPage.Model.ShouldBeOfType<SubclassTestModelForActivation>();
        }
    }

    public class TestModelForActivation : SimpleFubuPage<TestModelForActivation>
    {
    }

    public class SubclassTestModelForActivation : TestModelForActivation
    {
    }
}