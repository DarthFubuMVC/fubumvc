using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Core.Infrastructure;

namespace FubuMVC.Diagnostics.Features.Routes.Authorization
{
    public class GetHandler
    {
        private readonly BehaviorGraph _behaviorGraph;
        private readonly IAuthorizationDescriptor _authorizationDescriptor;

        public GetHandler(BehaviorGraph behaviorGraph, IAuthorizationDescriptor authorizationDescriptor)
        {
            _behaviorGraph = behaviorGraph;
            _authorizationDescriptor = authorizationDescriptor;
        }

        public AuthorizationModel Execute(AuthorizationRequestModel request)
        {
            var rules = new Cache<string, AuthorizationRuleModel>(r => new AuthorizationRuleModel(r));
            _behaviorGraph
                .Behaviors
                .Each(chain =>
                            {
                                var authorizor = _authorizationDescriptor.AuthorizorFor(chain);
                                authorizor
                                    .RulesDescriptions()
                                    .Each(r => rules[r].AddRoute(new AuthorizedRouteModel(chain.UniqueId, chain.Route == null ? "(no route)" : chain.Route.Pattern)));
                            });

            return new AuthorizationModel
                       {
                           Rules = rules
                       };
        }
    }
}