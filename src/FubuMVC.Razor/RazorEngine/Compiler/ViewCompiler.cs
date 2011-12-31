using System;
using System.Collections.Generic;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Rendering;
using RazorEngine.Templating;
using ITemplate = RazorEngine.Templating.ITemplate;

namespace FubuMVC.Razor.RazorEngine.Compiler
{
    public class ViewCompiler
    {
        public ViewCompiler()
        {
            GeneratedViewId = Guid.NewGuid();
        }

        public Guid GeneratedViewId { get; private set; }
        public ITemplateService TemplateService { get; set; }
        public ViewDescriptor Descriptor { get; set; }

        public IEnumerable<string> UseNamespaces { get; set; }

        public ITemplate CreateInstance()
        {
            var currentDescriptor = Descriptor;
            var sourceCode = currentDescriptor.ViewLoader.ViewFile.GetSourceCode();
            var returnTemplate = (IMayHaveLayout)TemplateService.CreateTemplate(sourceCode);
            returnTemplate.TemplateService = TemplateService;
            while(currentDescriptor.Master != null)
            {
                currentDescriptor = (ViewDescriptor)currentDescriptor.Master.Descriptor;
                var layoutTemplate = (IMayHaveLayout)TemplateService.CreateTemplate(currentDescriptor.ViewLoader.ViewFile.GetSourceCode());
                returnTemplate.Layout = layoutTemplate;
            }
            return returnTemplate;
        }
    }
}