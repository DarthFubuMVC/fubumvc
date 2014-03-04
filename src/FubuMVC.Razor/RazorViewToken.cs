using System;
using FubuCore;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Rendering;

namespace FubuMVC.Razor
{
    public class RazorViewToken : IViewToken
    {
        private readonly ViewDescriptor<IRazorTemplate> _descriptor;
        private readonly ITemplateFactory _templateFactory;

        public RazorViewToken(ViewDescriptor<IRazorTemplate> viewDescriptor, ITemplateFactory templateFactory)
        {
            _descriptor = viewDescriptor;
            _templateFactory = templateFactory;
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
            var currentDescriptor = _descriptor;
            var returnTemplate = _templateFactory.GetView(currentDescriptor);
            returnTemplate.OriginTemplate = _descriptor.Template;
            var currentTemplate = returnTemplate;

            while (currentDescriptor.Master != null && !partialOnly)
            {
                currentDescriptor = currentDescriptor.Master.Descriptor.As<ViewDescriptor<IRazorTemplate>>();

                var layoutTemplate = _templateFactory.GetView(currentDescriptor);
                layoutTemplate.OriginTemplate = returnTemplate.OriginTemplate;
                currentTemplate.UseLayout(layoutTemplate);
                currentTemplate = layoutTemplate;
            }

            return returnTemplate;
        }

        public string ProfileName { get; set; }

        public Type ViewType
        {
            get { return typeof (IRazorTemplate); }
        }

        public Type ViewModel
        {
            get { return _descriptor.ViewModel; }
        }

        public string Name()
        {
            return _descriptor.Name();
        }

        public string Namespace
        {
            get { return string.Empty; }
        }

        public override string ToString()
        {
            return _descriptor.RelativePath();
        }
    }
}