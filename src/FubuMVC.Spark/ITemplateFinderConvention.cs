using FubuMVC.Core.Packaging;
using FubuMVC.Core.View.Conventions;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    public class DefaultTemplateFinderConventions : ITemplateFinderConvention<Template>
    {
        public void Configure(TemplateFinder<Template> finder)
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