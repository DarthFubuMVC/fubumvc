using FubuCore.Binding;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class RecordingModelBinderWrapperTester : InteractionContext<RecordingModelBinderWrapper>
    {
        [Test]
        public void should_resolve_binder_and_log_selection()
        {
            MockFor<IModelBinderCache>()
                .Expect(c => c.BinderFor(typeof (SimpleModel)))
                .Return(StandardModelBinder.Basic());

            MockFor<IDebugReport>()
                .Expect(r => r.AddBindingDetail(new ModelBinderSelection
                                                    {
                                                        BinderType = typeof(StandardModelBinder),
                                                        ModelType = typeof(SimpleModel)
                                                    }));
            ClassUnderTest
                .BinderFor(typeof (SimpleModel))
                .ShouldBeOfType<StandardModelBinder>();

            VerifyCallsFor<IDebugReport>();
        }

        public class SimpleModel { }
    }
}