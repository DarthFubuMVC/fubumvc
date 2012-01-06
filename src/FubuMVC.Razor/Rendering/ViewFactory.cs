using FubuMVC.Razor.RazorModel;
using RazorEngine.Templating;
using ITemplate = RazorEngine.Templating.ITemplate;

namespace FubuMVC.Razor.Rendering
{
    public interface IViewFactory
    {
        IFubuRazorView GetView();
    }

    public class ViewFactory : IViewFactory
    {
        private readonly ViewDescriptor _viewDescriptor;
        private readonly ITemplateService _templateService;
        private readonly IViewModifierService _service;

        public ViewFactory(ViewDescriptor viewDescriptor, ITemplateServiceWrapper templateServiceWrapper, IViewModifierService service)
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
            if (shouldCreateNewInstance())
                return CompileNewInstance();

            return CreateExistingInstance();
        }

        private bool shouldCreateNewInstance()
        {
            return !_templateService.HasTemplate(_viewDescriptor.Template.GeneratedViewId.ToString()) 
                || !_viewDescriptor.IsCurrent();
        }

        private IFubuRazorView CreateExistingInstance()
        {
            var currentDescriptor = _viewDescriptor;
            var returnTemplate = (IFubuRazorView) _templateService.Resolve(currentDescriptor.Template.GeneratedViewId.ToString());
            var currentTemplate = returnTemplate;
            while (currentDescriptor.Master != null)
            {
                currentDescriptor = (ViewDescriptor) currentDescriptor.Master.Descriptor;
                var layoutTemplate = (IFubuRazorView) _templateService.Resolve(currentDescriptor.Template.GeneratedViewId.ToString());
                currentTemplate.Layout = (ITemplate) layoutTemplate;
                currentTemplate = layoutTemplate;
            }
            return returnTemplate;
        }

        private IFubuRazorView CompileNewInstance()
        {
            var currentDescriptor = _viewDescriptor;
            var sourceCode = currentDescriptor.ViewFile.GetSourceCode();
            var returnTemplate = (IFubuRazorView)_templateService.GetTemplate(sourceCode, currentDescriptor.Template.GeneratedViewId.ToString());

            var currentTemplate = returnTemplate;
            while(currentDescriptor.Master != null)
            {
                currentDescriptor = (ViewDescriptor)currentDescriptor.Master.Descriptor;
                var layoutTemplate = (IFubuRazorView)_templateService.GetTemplate(currentDescriptor.ViewFile.GetSourceCode(), currentDescriptor.Template.GeneratedViewId.ToString());
                currentTemplate.Layout = (ITemplate)layoutTemplate;
                currentTemplate = layoutTemplate;
            }
            return returnTemplate;
        }
    }
}