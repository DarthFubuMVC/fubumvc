using FubuMVC.Core;

namespace FubuMVC.NewDiagnostics
{
    public class DiagnosticRegistry : FubuPackageRegistry
    {
        public DiagnosticRegistry() : base("_fubu2")
        {
            Actions.IncludeTypesNamed(x => x.EndsWith("Endpoint"));
            Views.TryToAttachWithDefaultConventions();
        }
    }
}