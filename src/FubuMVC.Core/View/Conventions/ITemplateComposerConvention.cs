using System;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Core.View.Conventions
{
    public interface ITemplateComposerConvention<T> where T : ITemplateFile
    {
        void Configure(TemplateComposer<T> composer);
    }

    public class LambdaTemplateComposerConvention<T> : ITemplateComposerConvention<T> where T : ITemplateFile
    {
        private readonly Action<TemplateComposer<T>> _configure;

        public LambdaTemplateComposerConvention(Action<TemplateComposer<T>> configure)
        {
            _configure = configure;
        }

        public void Configure(TemplateComposer<T> composer)
        {
            _configure(composer);
        }
    }
}