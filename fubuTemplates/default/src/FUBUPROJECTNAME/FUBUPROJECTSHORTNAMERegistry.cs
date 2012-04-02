using FubuMVC.Core;
using FubuMVC.Spark;

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
                .IgnoreControllerNamespaceEntirely();

            Import<SparkEngine>();

            Views
                .TryToAttachWithDefaultConventions();
        }
    }
}
