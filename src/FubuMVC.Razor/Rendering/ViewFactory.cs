using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.Rendering
{
    public class ViewFactory : IViewFactory
    {
        private readonly IRazorDescriptor _viewDescriptor;
        private readonly IFubuTemplateService _templateService;
        private readonly IViewModifierService<IFubuRazorView> _service;

        public ViewFactory(IRazorDescriptor viewDescriptor, ITemplateServiceWrapper templateServiceWrapper, IViewModifierService<IFubuRazorView> service)
        {
            _viewDescriptor = viewDescriptor;
            _templateService = templateServiceWrapper.TemplateService;
            _service = service;
        }

        public IRenderableView GetView()
        {
            return CreateInstance();
        }

        public IRenderableView GetPartialView()
        {
            return CreateInstance(true);
        }

        private IFubuRazorView CreateInstance(bool partialOnly = false)
        {
            var currentDescriptor = _viewDescriptor;
            var returnTemplate = _templateService.GetView(currentDescriptor);
            var currentTemplate = returnTemplate;
            while (currentDescriptor.Master != null && !partialOnly)
            {
                currentDescriptor = currentDescriptor.Master.Descriptor;
                var layoutTemplate = _templateService.GetView(currentDescriptor);
                currentTemplate.Layout = layoutTemplate;
                currentTemplate = layoutTemplate;
            }
            returnTemplate = _service.Modify(returnTemplate);
            return returnTemplate;
        }
    }
}