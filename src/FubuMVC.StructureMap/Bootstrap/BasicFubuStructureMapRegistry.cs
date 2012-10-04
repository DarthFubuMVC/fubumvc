using FubuMVC.Core;

namespace FubuMVC.StructureMap.Bootstrap
{
    public class BasicFubuStructureMapRegistry : FubuRegistry
    {
        public BasicFubuStructureMapRegistry(bool enableDiagnostics, string controllerAssemblyName)
        {
            Applies.ToAssembly(controllerAssemblyName);

            Actions
                .IncludeTypesNamed(x => x.EndsWith("Controller"));

            Routes
                .IgnoreControllerNamespaceEntirely();
        }
    }
}