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
            Register(composer => composer
                .AddBinder(new ViewDescriptorBinder<IRazorTemplate>(new RazorTemplateSelector()))
                .AddBinder<GenericViewModelBinder<IRazorTemplate>>()
                .AddBinder<ViewModelBinder<IRazorTemplate>>()
                .Apply<ViewPathPolicy<IRazorTemplate>>());
        }

        public void Register(Action<TemplateComposer<IRazorTemplate>> alteration)
        {
            _configurations += alteration;
        }

        public void ResetComposerConfiguration()
        {
            _configurations = new CompositeAction<TemplateComposer<IRazorTemplate>>();
        }

        public void Configure(TemplateComposer<IRazorTemplate> composer)
        {
            _configurations.Do(composer);
        }

        public FileSet Search { get; private set; }
    }
}