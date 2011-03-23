using FubuCore.Binding;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.StructureMap;
using FubuMVC.Tests.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class when_resolving_successfully : InteractionContext<RecordingObjectResolver>
    {
        private BindResult result;
        private InMemoryBindingContext context;

        protected override void beforeEach()
        {
            result = new BindResult
            {
                Value = new object()
            };
            context = new InMemoryBindingContext();
            MockFor<ObjectResolver>().Expect(x => x.BindModel(typeof (BinderTarget), context)).Return(result);

            ClassUnderTest.BindModel(typeof (BinderTarget), context).ShouldBeTheSameAs(result);
        }

        [Test]
        public void should_report_the_end_of_the_binding()
        {
            MockFor<IDebugReport>().AssertWasCalled(x => x.EndModelBinding(result.Value));
        }

        [Test]
        public void should_start_a_new_binding_report()
        {
            MockFor<IDebugReport>().AssertWasCalled(x => x.StartModelBinding(typeof (BinderTarget)));
        }
    }
}