using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.Rendering
{
    public class ViewFactory : IViewFactory
    {
        private readonly ViewDescriptor<IRazorTemplate> _viewDescriptor;
        private readonly IFubuTemplateService _templateService;
        private readonly IViewModifierService<IFubuRazorView> _service;

        public ViewFactory(ViewDescriptor<IRazorTemplate> viewDescriptor, IFubuTemplateService templateService, IViewModifierService<IFubuRazorView> service)
        {
            _viewDescriptor = viewDescriptor;
            _templateService = templateService;
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
            returnTemplate.OriginTemplate = _viewDescriptor.Template;
            var currentTemplate = returnTemplate;
            while (currentDescriptor.Master != null && !partialOnly)
            {
                currentDescriptor = currentDescriptor.Master.Descriptor.As<ViewDescriptor<IRazorTemplate>>();
                var layoutTemplate = _templateService.GetView(currentDescriptor);
                layoutTemplate.OriginTemplate = returnTemplate.OriginTemplate;
                currentTemplate.UseLayout(layoutTemplate);
                currentTemplate = layoutTemplate;
            }
            returnTemplate = _service.Modify(returnTemplate);
            return returnTemplate;
        }
    }
}