﻿using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.Rendering
{
    [TestFixture]
    public class RenderActionTester : InteractionContext<RenderAction>
    {
        private IFubuRazorView _view;
        private IViewFactory _viewFactory;
        protected override void beforeEach()
        {
            _viewFactory = MockFor<IViewFactory>();
            _view = MockFor<IFubuRazorView>();
        }

        [Test]
        public void render_uses_view_from_factory_and_calls_to_render_method()
        {
            _view.Expect(x => x.Render());
            _viewFactory.Stub(x => x.GetView()).Return(_view);
            ClassUnderTest.Render();
            _view.VerifyAllExpectations();
        }

        [Test]
        public void render_partial_uses_partial_view_from_factory_and_calls_to_render_method()
        {
            _view.Expect(x => x.RenderPartial());
            _viewFactory.Stub(x => x.GetView()).Return(_view);
            ClassUnderTest.RenderPartial();
            _view.VerifyAllExpectations();
        }
    }
}