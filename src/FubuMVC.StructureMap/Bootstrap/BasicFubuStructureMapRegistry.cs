using FubuMVC.Core;

namespace FubuMVC.StructureMap.Bootstrap
{
    public class BasicFubuStructureMapRegistry : FubuRegistry
    {
        public BasicFubuStructureMapRegistry(bool enableDiagnostics, string controllerAssemblyName)
        {
            IncludeDiagnostics(enableDiagnostics);

            Applies.ToAssembly(controllerAssemblyName);

            Actions
                .IncludeTypesNamed(x => x.EndsWith("Controller"));

            Routes
                .IgnoreControllerNamespaceEntirely();

            Views.TryToAttach(x =>
            {
                x.by_ViewModel_and_Namespace_and_MethodName();
                x.by_ViewModel_and_Namespace();
                x.by_ViewModel();
            });
        }
    }
}