using System;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.FileSystem;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using RazorEngine.Templating;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.Rendering
{
    [TestFixture]
    public class ViewFactoryTester : InteractionContext<ViewFactory>
    {
        private ITemplateService _templateService;
        private IViewModifierService<IFubuRazorView> _service;

        private FubuRazorView _entryView;
        private IFubuRazorView _serviceView;

        protected override void beforeEach()
        {
            var source = "<h1>hi</h1>";
            var viewId = Guid.NewGuid();
            _service = MockFor<IViewModifierService<IFubuRazorView>>();
            _templateService = MockFor<ITemplateService>();
            var viewFile = MockFor<IViewFile>();
            viewFile.Expect(x => x.GetSourceCode()).Return(source);
            var descriptor = MockFor<IRazorDescriptor>();
            descriptor.Expect(x => x.Template.GeneratedViewId).Return(viewId);
            descriptor.Expect(x => x.ViewFile).Return(viewFile);

            Services.Inject<ITemplateServiceWrapper>(new TemplateServiceWrapper(new FubuTemplateService(new TemplateRegistry<IRazorTemplate>(), _templateService)));
            _entryView = MockRepository.GenerateMock<StubView>();
            _serviceView = MockRepository.GenerateMock<IFubuRazorView>();

            _templateService.Expect(x => x.HasTemplate(Arg.Is(viewId.ToString()))).Return(false);
            _templateService.Expect(x => x.GetTemplate(Arg.Is(source), Arg.Is(viewId.ToString()))).Return(_entryView);
            _service.Expect(x => x.Modify(_entryView)).Return(_serviceView);
        }

        [Test]
        public void getview_returns_and_applies_the_service_modifications()
        {
            ClassUnderTest.GetView().ShouldEqual(_serviceView);
            _templateService.VerifyAllExpectations();
            _service.VerifyAllExpectations();
        }
    }
}