using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids.Builders
{
    public class DefaultBehaviorChainColumnBuilder : IGridColumnBuilder<BehaviorChain>
    {
        private readonly IHttpConstraintResolver _resolver;
        private readonly IUrlRegistry _urls;
        private readonly IAuthorizationDescriptor _descriptor;

        public DefaultBehaviorChainColumnBuilder(IHttpConstraintResolver resolver, IUrlRegistry urls, IAuthorizationDescriptor descriptor)
        {
            _resolver = resolver;
            _descriptor = descriptor;
            _urls = urls;
        }

        public IEnumerable<JsonGridColumn> ColumnsFor(BehaviorChain target)
        {
            var route = target.RoutePattern;
            if(target.Route == null)
            {
                route = "(no route)";
            }
            else if(target.RoutePattern.IsEmpty())
            {
                route = "(default)";
            }

            var rules = _descriptor.AuthorizorFor(target).RulesDescriptions();
            var authorization = rules.Any() ? rules.Join(", ") : "none";
            return new List<JsonGridColumn>
                       {
                           new JsonGridColumn {Name = "Route", Value = route},
                           new JsonGridColumn {Name = "Constraints", Value = _resolver.Resolve(target)},
                           new JsonGridColumn {Name = "Action", Value = target.FirstCallDescription},
                           new JsonGridColumn {Name = "InputModel", Value = target.InputType() == null ? string.Empty : target.InputType().FullName},
                           new JsonGridColumn {Name = "OutputModel", Value = target.ActionOutputType() == null ? string.Empty : target.ActionOutputType().FullName},
                           new JsonGridColumn {Name = "ChainUrl", IsIdentifier = true, Value = _urls.UrlFor(new ChainRequest { Id = target.UniqueId }), IsHidden = true, HideFilter = true},
                           new JsonGridColumn {Name = "UrlCategory", Value = target.UrlCategory.Category, IsHidden = true},
                           new JsonGridColumn {Name = "Provenance", Value = target.Origin, IsHidden = true},
                           new JsonGridColumn {Name = "Authorization", Value = authorization, IsHidden = true}
                       };
        }
    }
}