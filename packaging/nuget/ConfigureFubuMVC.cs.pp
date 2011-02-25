using FubuMVC.Core;

namespace $rootnamespace$
{
    public class ConfigureFubuMVC : FubuRegistry
    {
        public ConfigureFubuMVC()
        {
            IncludeDiagnostics(true);

            Actions.IncludeClassesSuffixedWithController();
            Routes.IgnoreControllerNamespaceEntirely();
        }
    }
}