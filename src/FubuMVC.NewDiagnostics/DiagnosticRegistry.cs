using FubuMVC.Core;
using FubuMVC.Spark;

namespace FubuMVC.NewDiagnostics
{
    public class DiagnosticRegistry : FubuPackageRegistry
    {
        public DiagnosticRegistry() : base("_fubu2")
        {
            Views.TryToAttachWithDefaultConventions();
        }
    }
}