using System;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
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