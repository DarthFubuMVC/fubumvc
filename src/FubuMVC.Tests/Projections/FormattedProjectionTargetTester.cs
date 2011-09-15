using FubuCore;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class when_retrieving_a_value_for_an_accessor : InteractionContext<FormattedProjectionTarget>
    {
        private string theInnerValue;
        private Accessor theAccessor;
        private string theFormattedValue;
        private object theValueReturnedFromTheFormattedProjectionTarget;

        protected override void beforeEach()
        {
            theAccessor = ReflectionHelper.GetAccessor<SimpleClass>(x => x.Name);
            theInnerValue = "something";
            theFormattedValue = "formatted";

            MockFor<IProjectionTarget>().Stub(x => x.ValueFor(theAccessor)).Return(theInnerValue);

            // TODO -- think you'll want a custom service here
            MockFor<IDisplayFormatter>().Stub(x => x.GetDisplay(theAccessor, theInnerValue))
                .Return(theFormattedValue);

            theValueReturnedFromTheFormattedProjectionTarget = ClassUnderTest.ValueFor(theAccessor);
        }

        [Test]
        public void the_value_returned_from_the_formatted_projection_target_should_be_the_inner_value_processed_through_display_formatter()
        {
            theValueReturnedFromTheFormattedProjectionTarget.ShouldEqual(theFormattedValue);
        }
    }
}