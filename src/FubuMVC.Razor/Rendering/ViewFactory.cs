using System;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.Rendering
{
    public class ViewFactory : IViewFactory
    {
        private readonly ViewDescriptor<IRazorTemplate> _viewDescriptor;
        private readonly ITemplateFactory _templateFactory;
        private readonly IViewModifierService<IFubuRazorView> _service;

        public ViewFactory(ViewDescriptor<IRazorTemplate> viewDescriptor, ITemplateFactory templateFactory, IViewModifierService<IFubuRazorView> service)
        {
            _viewDescriptor = viewDescriptor;
            _templateFactory = templateFactory;
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
            var returnTemplate = _templateFactory.GetView(currentDescriptor);
            returnTemplate.OriginTemplate = _viewDescriptor.Template;
            var currentTemplate = returnTemplate;
            while (currentDescriptor.Master != null && !partialOnly)
            {
                currentDescriptor = currentDescriptor.Master.Descriptor.As<ViewDescriptor<IRazorTemplate>>();
                var layoutTemplate = _templateFactory.GetView(currentDescriptor);
                layoutTemplate.OriginTemplate = returnTemplate.OriginTemplate;
                currentTemplate.UseLayout(layoutTemplate);
                currentTemplate = layoutTemplate;
            }
            returnTemplate = _service.Modify(returnTemplate);
            return returnTemplate;
        }

        public void Describe(Description description)
        {
            description.Title = "Razor view " + _viewDescriptor.FullName();
        }
    }
}