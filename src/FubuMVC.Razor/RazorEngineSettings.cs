using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Rendering;

namespace FubuMVC.Razor
{
    public class RazorEngineSettings
    {
        private CompositeAction<TemplateComposer<IRazorTemplate>> _configurations = new CompositeAction<TemplateComposer<IRazorTemplate>>();

        public RazorEngineSettings()
        {
            defaultSearch();
            defaultComposer();
            defaultTemplateType();
        }

        private void defaultTemplateType()
        {
            BaseTemplateType = typeof(FubuRazorView);
        }

        private void defaultSearch()
        {
            Search = new FileSet { DeepSearch = true };
            Search.AppendInclude("*cshtml");
            Search.AppendInclude("*vbhtml");     
            Search.AppendExclude("bin/*.*");
            Search.AppendExclude("obj/*.*");
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

        public void UseBaseTemplateType<TTemplate>() where TTemplate : FubuRazorView
        {
            BaseTemplateType = typeof(TTemplate);
        }

        public FileSet Search { get; private set; }

        public Type BaseTemplateType { get; private set; }
    }
}