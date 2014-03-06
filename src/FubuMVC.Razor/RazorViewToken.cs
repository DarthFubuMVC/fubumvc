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
        private readonly IRazorTemplate _template;
        private readonly ITemplateFactory _templateFactory;

        public RazorViewToken(IRazorTemplate template, ITemplateFactory templateFactory)
        {
            _template = template;
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
            var currentDescriptor = _template;
            var returnTemplate = _templateFactory.GetView(currentDescriptor);
            returnTemplate.OriginTemplate = _template;
            var currentTemplate = returnTemplate;

            while (currentDescriptor.Master != null && !partialOnly)
            {
                currentDescriptor = currentDescriptor.Master.As<IRazorTemplate>();

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
            get { return _template.ViewModel; }
        }

        public string Name()
        {
            return _template.Name();
        }

        public string Namespace
        {
            get { return string.Empty; }
        }

        public override string ToString()
        {
            return _template.RelativePath();
        }

        public void AttachViewModels(ViewTypePool types, ITemplateLogger logger)
        {
            _template.AttachViewModels(types, logger);
        }
    }
}