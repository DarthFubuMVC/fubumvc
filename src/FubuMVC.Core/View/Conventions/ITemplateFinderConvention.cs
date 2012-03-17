using System;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Core.View.Conventions
{
    public interface ITemplateFinderConvention<T> where T : ITemplateFile, new()
    {
        void Configure(TemplateFinder<T> finder);
    }

    public class LambdaTemplateFinderConvention<T> : ITemplateFinderConvention<T> where T : ITemplateFile, new()
    {
        private readonly Action<TemplateFinder<T>> _configure;

        public LambdaTemplateFinderConvention(Action<TemplateFinder<T>> configure)
        {
            _configure = configure;
        }

        public void Configure(TemplateFinder<T> finder)
        {
            _configure(finder);
        }
    }
}