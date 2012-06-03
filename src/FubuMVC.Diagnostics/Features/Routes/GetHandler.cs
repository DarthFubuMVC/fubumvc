using FubuCore;
using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Features.Routes
{
    [MarkedForTermination("Just unnecessary code noise")]
    public class GetHandler
    {
        private readonly IModelBuilder<RoutesModel> _routeBuilder;

        public GetHandler(IModelBuilder<RoutesModel> routeBuilder)
        {
            _routeBuilder = routeBuilder;
        }

        public RoutesModel Execute(DefaultRouteRequestModel request)
        {
            return _routeBuilder.Build();
        }
    }
}