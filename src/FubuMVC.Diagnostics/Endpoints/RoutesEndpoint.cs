using FubuMVC.Diagnostics.Models.Routes;

namespace FubuMVC.Diagnostics.Endpoints
{
    public class RoutesEndpoint
    {
        public RoutesModel Get(RouteRequestModel request)
        {
            return new RoutesModel();
        }
    }
}