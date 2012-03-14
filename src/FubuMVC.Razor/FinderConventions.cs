using FubuMVC.Core.Packaging;
using FubuMVC.Core.View.Conventions;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public class DefaultRazorTemplateFinderConventions : ITemplateFinderConvention<Template>
    {
        public void Configure(TemplateFinder<Template> finder)
        {
            finder.IncludeFile("*cshtml");
            finder.IncludeFile("*vbhtml");

            finder.ExcludeHostDirectory(FubuMvcPackageFacility.FubuPackagesFolder);
            finder.ExcludeHostDirectory(FubuMvcPackageFacility.FubuPackagesFolder, FubuMvcPackageFacility.FubuContentFolder);
            finder.ExcludeHostDirectory(FubuMvcPackageFacility.FubuContentFolder);
        }
    }
}