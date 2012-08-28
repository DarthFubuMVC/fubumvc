using System;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public class RazorEngineSettings
    {
        private CompositeAction<TemplateComposer<IRazorTemplate>> _configurations = new CompositeAction<TemplateComposer<IRazorTemplate>>();

        public RazorEngineSettings()
        {
            defaultSearch();
            defaultComposer();
        }

        private void defaultSearch()
        {
            Search = new FileSet { DeepSearch = true };
            Search.AppendInclude("*cshtml");
            Search.AppendInclude("*vbhtml");            
        }

        private void defaultComposer()
        {
            ConfigureComposer(composer => composer
                .AddBinder(new ViewDescriptorBinder<IRazorTemplate>(new RazorTemplateSelector()))
                .AddBinder<GenericViewModelBinder<IRazorTemplate>>()
                .AddBinder<ViewModelBinder<IRazorTemplate>>()
                .Apply<ViewPathPolicy<IRazorTemplate>>());
        }

        public void ConfigureComposer(Action<TemplateComposer<IRazorTemplate>> config)
        {
            _configurations += config;
        }

        // TODO: Ask around
        public void ResetComposerConfiguration()
        {
            _configurations = new CompositeAction<TemplateComposer<IRazorTemplate>>();
        }

        public FileSet Search { get; private set; }
        public Action<TemplateComposer<IRazorTemplate>> ComposerConfiguration { get { return _configurations.Do; } } 
    }
}