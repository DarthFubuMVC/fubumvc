using FubuMVC.Diagnostics.Models;

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