using System;
using System.IO;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class NestedOutputTester : InteractionContext<NestedOutput>
    {
		private IFubuSparkView _view;
		
		protected override void beforeEach()
        {
            _view = MockFor<IFubuSparkView>();
        }

		
        [Test]
        public void if_the_writer_has_not_been_set_is_active_returns_false()
        {
            ClassUnderTest.IsActive().ShouldBeFalse();
        }

        [Test]
        public void if_the_view_has_been_set_is_active_returns_true()
        {
            ClassUnderTest.SetView(() => _view);
            ClassUnderTest.IsActive().ShouldBeTrue();
        }

        [Test]
        public void writer_executes_the_delegate_passed_on_set_view()
        {
            Func<IFubuSparkView> viewFunc = () => _view;
            ClassUnderTest.SetView(viewFunc);
            ClassUnderTest.View.ShouldEqual(_view);
        }
    }
}