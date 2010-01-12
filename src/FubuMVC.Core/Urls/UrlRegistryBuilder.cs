using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Urls
{
    public class UrlRegistryBuilder : IRouteVisitor
    {
        private readonly IUrlRegistration _registration;

        public UrlRegistryBuilder(IUrlRegistration registration)
        {
            _registration = registration;
        }

        public void VisitRoute(IRouteDefinition route, BehaviorChain chain)
        {
            ActionCall action = chain.FirstCall();
            if (action == null) return;

            if (action.HasInput)
            {
                // TODO:  throw if route is null;
                var model = (IModelUrl) route;
                _registration.AddModel(model);
            }

            var actionUrl = new ActionUrl(route, action);
            _registration.AddAction(actionUrl);
        }
    }
}