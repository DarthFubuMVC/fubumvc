using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Models;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class when_resolving_successfully : InteractionContext<RecordingObjectResolver>
    {
        private BindResult result;
        private InMemoryRequestData data;

        protected override void beforeEach()
        {
            result = new BindResult
            {
                Value = new object()
            };
            data = new InMemoryRequestData();
            MockFor<ObjectResolver>().Expect(x => x.BindModel(typeof (BinderTarget), data)).Return(result);

            ClassUnderTest.BindModel(typeof (BinderTarget), data).ShouldBeTheSameAs(result);
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