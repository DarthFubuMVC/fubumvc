using FubuCore.Reflection;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Elements;
using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Elements
{
    [TestFixture]
    public class ElementRequestActivatorTester : InteractionContext<ElementRequestActivator>
    {
        [Test]
        public void matches_negative()
        {
            ClassUnderTest.Matches(typeof(TagRequest)).ShouldBeFalse();
        }

        [Test]
        public void matches_positive()
        {
            ClassUnderTest.Matches(typeof(ElementRequest)).ShouldBeTrue();
            ClassUnderTest.Matches(typeof(SpecialElementRequest)).ShouldBeTrue();
        }

    }

    [TestFixture]
    public class when_activating_when_the_request_already_has_the_model : InteractionContext<ElementRequestActivator>
    {
        private ElementRequest theElementRequest;

        protected override void beforeEach()
        {
            theElementRequest = ElementRequest.For<Address>(new Address(), x => x.Address1);

            MockFor<IElementNamingConvention>().Stub(
                x => x.GetName(theElementRequest.HolderType(), theElementRequest.Accessor))
                .Return("the element name");

            ClassUnderTest.Activate(theElementRequest);
        }

        [Test]
        public void should_place_the_element_name_on_the_request()
        {
            theElementRequest.ElementId.ShouldEqual("the element name");
        }

        [Test]
        public void should_leave_the_model_alone_on_the_request()
        {
            theElementRequest.Model.ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class when_activating_when_the_request_does_not_already_has_the_model : InteractionContext<ElementRequestActivator>
    {
        private ElementRequest theElementRequest;
        private Address theAddress;

        protected override void beforeEach()
        {
            theElementRequest = ElementRequest.For<Address>(x => x.Address1);

            theAddress = new Address();
            MockFor<IFubuRequest>().Stub(x => x.Get(theElementRequest.HolderType()))
                .Return(theAddress);

            MockFor<IElementNamingConvention>().Stub(
                x => x.GetName(theElementRequest.HolderType(), theElementRequest.Accessor))
                .Return("the element name");

            ClassUnderTest.Activate(theElementRequest);
        }

        [Test]
        public void should_place_the_element_name_on_the_request()
        {
            theElementRequest.ElementId.ShouldEqual("the element name");
        }

        [Test]
        public void should_attach_the_model_from_fubu_request()
        {
            theElementRequest.Model.ShouldBeTheSameAs(theAddress);
        }
    }

    public class SpecialElementRequest : ElementRequest
    {
        public SpecialElementRequest(Accessor accessor) : base(accessor)
        {
        }
    }
}