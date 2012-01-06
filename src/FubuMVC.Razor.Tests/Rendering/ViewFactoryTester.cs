using System;
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
        private IViewModifierService _service;

        private FubuRazorView _entryView;
        private IFubuRazorView _serviceView;

        protected override void beforeEach()
        {
            var viewId = Guid.NewGuid();
            _service = MockFor<IViewModifierService>();
            _templateService = MockFor<ITemplateService>();
            var descriptor = MockFor<ViewDescriptor>();
            descriptor.Expect(x => x.Template.GeneratedViewId).Return(viewId);

            Services.Inject<ITemplateServiceWrapper>(new TemplateServiceWrapper(_templateService));
            _entryView = MockRepository.GenerateMock<FubuRazorView>();
            _serviceView = MockRepository.GenerateMock<IFubuRazorView>();

            _templateService.Expect(x => x.HasTemplate(Arg.Is(viewId.ToString()))).Return(false);
            _templateService.Expect(x => x.GetTemplate(Arg.Is("<h1>hi</h1>"), viewId.ToString())).Return(_entryView);
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