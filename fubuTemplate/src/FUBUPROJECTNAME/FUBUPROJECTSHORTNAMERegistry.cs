using FubuMVC.Core;
using FubuMVC.Spark;
using FUBUPROJECTNAME.Controllers.Home;

namespace FUBUPROJECTNAME
{
    public class FUBUPROJECTSHORTNAMERegistry : FubuRegistry
    {
        public FUBUPROJECTSHORTNAMERegistry()
        {
            IncludeDiagnostics(true);

            Applies
                .ToThisAssembly();

            Actions
                .IncludeClassesSuffixedWithController();

            Routes
                .HomeIs<HomeController>(c => c.Welcome())
                .IgnoreControllerNamespaceEntirely();
			
            this.UseSpark();

            Views
                .TryToAttachWithDefaultConventions();

            HtmlConvention<FUBUPROJECTSHORTNAMEHtmlConventions>();
        }
    }
}
