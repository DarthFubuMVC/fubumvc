using System.IO;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class DefaultRenderActionTester : InteractionContext<DefaultRenderAction>
    {
        private IFubuSparkView _view;
        private NestedOutput _nestedOutput;

        protected override void beforeEach()
        {
            var viewOutput = MockFor<ViewOutput>();

            _nestedOutput = new NestedOutput();

            _view = MockFor<IFubuSparkView>();

            var viewFactory = MockFor<IViewFactory>();
            viewFactory.Stub(x => x.GetView()).Return(_view);

            _view.Stub(x => x.Output).Return(new StringWriter());
            _view.Expect(x => x.RenderView(viewOutput));

            Services.Inject(_nestedOutput);
            Services.Inject(viewFactory);
            Services.Inject(viewOutput);

            ClassUnderTest.Render();
        }

        [Test]
        public void renders_view_from_factory_using_injected_view_output()
        {
            _view.VerifyAllExpectations();
        }

        [Test]
        public void sets_nested_output_writer_as_view_output()
        {
            _nestedOutput.IsActive();
            _nestedOutput.Writer.ShouldEqual(_view.Output);
        }
    }
}