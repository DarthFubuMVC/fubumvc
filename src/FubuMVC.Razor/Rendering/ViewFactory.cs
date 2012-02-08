using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.Rendering
{
    public interface IViewFactory
    {
        IFubuRazorView GetView();
    }

    public class ViewFactory : IViewFactory
    {
        private readonly IRazorDescriptor _viewDescriptor;
        private readonly IFubuTemplateService _templateService;
        private readonly IViewModifierService _service;

        public ViewFactory(IRazorDescriptor viewDescriptor, ITemplateServiceWrapper templateServiceWrapper, IViewModifierService service)
        {
            _viewDescriptor = viewDescriptor;
            _templateService = templateServiceWrapper.TemplateService;
            _service = service;
        }

        public IFubuRazorView GetView()
        {
            var view =  CreateInstance();
            view = _service.Modify(view);
            return view;
        }

        private IFubuRazorView CreateInstance()
        {
            var currentDescriptor = _viewDescriptor;
            var returnTemplate = _templateService.GetView(currentDescriptor);
            var currentTemplate = returnTemplate;
            while (currentDescriptor.Master != null)
            {
                currentDescriptor = currentDescriptor.Master.Descriptor;
                var layoutTemplate = _templateService.GetView(currentDescriptor);
                currentTemplate.Layout = layoutTemplate;
                currentTemplate = layoutTemplate;
            }
            return returnTemplate;
        }
    }
}