using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;
using FubuMVC.Tests.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class RecordingFubuRequestTester : InteractionContext<RecordingFubuRequest>
    {
        [Test]
        public void setting_target_by_object()
        {
            var target = new BinderTarget();
            ClassUnderTest.SetObject(target);

            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.AddDetails(new SetValueReport
            //{
            //    Type = typeof (BinderTarget),
            //    Value = target
            //}));

            ClassUnderTest.Get<BinderTarget>().ShouldBeTheSameAs(target);
        }

        [Test]
        public void setting_target_by_type()
        {
            var target = new BinderTarget();
            ClassUnderTest.Set(target);

            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.AddDetails(new SetValueReport
            //{
            //    Type = typeof (BinderTarget),
            //    Value = target
            //}));

            ClassUnderTest.Get<BinderTarget>().ShouldBeTheSameAs(target);
        }
    }
}