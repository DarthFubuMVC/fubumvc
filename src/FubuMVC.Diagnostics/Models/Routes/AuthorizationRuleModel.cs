using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models.Routes
{
    public class AuthorizationRuleModel
    {
        private readonly IList<AuthorizedRouteModel> _routes = new List<AuthorizedRouteModel>();

        public AuthorizationRuleModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public IEnumerable<AuthorizedRouteModel> Routes { get { return _routes; } }

        public void AddRoute(AuthorizedRouteModel route)
        {
            _routes.Fill(route);
        }
    }
}