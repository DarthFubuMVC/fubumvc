using System;
using FubuMVC.Core.Packaging;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    public interface ITemplateFinderConvention
    {
        void Configure(TemplateFinder finder);
    }

    public class LambdaTemplateFinderConvention : ITemplateFinderConvention
    {
        private readonly Action<TemplateFinder> _configure;

        public LambdaTemplateFinderConvention(Action<TemplateFinder> configure)
        {
            _configure = configure;
        }

        public void Configure(TemplateFinder finder)
        {
            _configure(finder);
        }
    }

    public class DefaultTemplateFinderConventions : ITemplateFinderConvention
    {
        public void Configure(TemplateFinder finder)
        {
            finder.IncludeFile("*spark");
            // TODO: This is not automatically synched with what the attacher looks for.
            finder.IncludeFile("bindings.xml");

            finder.ExcludeHostDirectory(FubuMvcPackageFacility.FubuPackagesFolder);
            finder.ExcludeHostDirectory(FubuMvcPackageFacility.FubuPackagesFolder, FubuMvcPackageFacility.FubuContentFolder);
            finder.ExcludeHostDirectory(FubuMvcPackageFacility.FubuContentFolder);
        }
    }
}