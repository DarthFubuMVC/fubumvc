using System;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public interface ITemplateComposerConvention
    {
        void Configure(TemplateComposer composer);
    }

    public class LambdaTemplateComposerConvention : ITemplateComposerConvention
    {
        private readonly Action<TemplateComposer> _configure;

        public LambdaTemplateComposerConvention(Action<TemplateComposer> configure)
        {
            _configure = configure;
        }

        public void Configure(TemplateComposer composer)
        {
            _configure(composer);
        }
    }
}