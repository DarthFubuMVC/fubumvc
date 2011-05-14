using System.IO;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class DefaultRenderActionTester : InteractionContext<DefaultRenderAction>
    {
        private IFubuSparkView _view;

        protected override void beforeEach()
        {
            var viewOutput = MockFor<ViewOutput>();
            _view = MockFor<IFubuSparkView>();

            var viewFactory = MockFor<IViewFactory>();
            viewFactory.Stub(x => x.GetView()).Return(_view);

            _view.Stub(x => x.Output).Return(new StringWriter());
			_view.Stub(x => x.Content).Return(new Dictionary<string, TextWriter>());
            _view.Expect(x => x.RenderView(viewOutput));

            ClassUnderTest.Render();
        }

        [Test]
        public void renders_view_from_factory_using_injected_view_output()
        {
            _view.VerifyAllExpectations();
        }
		
		// TODO : tests for Content being cleared missing
    }
}