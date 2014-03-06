using System;
using FubuCore;
using FubuMVC.Razor.Rendering;

namespace FubuMVC.Razor
{
    public class RazorEngineSettings
    {
        public RazorEngineSettings()
        {
            defaultSearch();
            defaultTemplateType();
        }

        private void defaultTemplateType()
        {
            BaseTemplateType = typeof (FubuRazorView);
        }

        private void defaultSearch()
        {
            Search = new FileSet {DeepSearch = true};
            Search.AppendInclude("*cshtml");
            Search.AppendInclude("*vbhtml");
            Search.AppendExclude("bin/*.*");
            Search.AppendExclude("obj/*.*");
        }

        public void UseBaseTemplateType<TTemplate>() where TTemplate : FubuRazorView
        {
            BaseTemplateType = typeof (TTemplate);
        }

        public FileSet Search { get; private set; }

        public Type BaseTemplateType { get; private set; }
    }
}