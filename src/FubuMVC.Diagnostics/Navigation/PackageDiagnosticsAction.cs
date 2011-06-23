using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Navigation
{
    public class PackageDiagnosticsAction : NavigationItemBase
    {
        public PackageDiagnosticsAction(BehaviorGraph graph, IEndpointService endpointService)
            : base(graph, endpointService)
        {
        }

        public override string Text()
        {
            return "Package Diagnostics";
        }

        protected override object inputModel()
        {
            return new PackageDiagnosticsRequestModel();
        }
    }
}