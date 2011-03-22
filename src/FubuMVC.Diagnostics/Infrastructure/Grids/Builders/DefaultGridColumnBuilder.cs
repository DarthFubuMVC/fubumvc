using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids.Builders
{
    public class DefaultGridColumnBuilder : IGridColumnBuilder<BehaviorChain>
    {
        private readonly IHttpConstraintResolver _resolver;
        private readonly IUrlRegistry _urls;

        public DefaultGridColumnBuilder(IHttpConstraintResolver resolver, IUrlRegistry urls)
        {
            _resolver = resolver;
            _urls = urls;
        }

        public IEnumerable<JsonGridColumn> ColumnsFor(BehaviorChain target)
        {
            return new List<JsonGridColumn>
                       {
                           new JsonGridColumn {IsIdentifier = true, Name = "Id", Value = target.UniqueId.ToString()},
                           new JsonGridColumn {Name = "Route", Value = target.RoutePattern},
                           new JsonGridColumn {Name = "Constraints", Value = _resolver.Resolve(target)},
                           new JsonGridColumn {Name = "Action", Value = target.FirstCallDescription},
                           new JsonGridColumn {Name = "InputModel", Value = target.InputType() == null ? string.Empty : target.InputType().FullName},
                           new JsonGridColumn {Name = "OutputModel", Value = target.ActionOutputType() == null ? string.Empty : target.ActionOutputType().FullName},
                           new JsonGridColumn {Name = "ChainUrl", Value = _urls.UrlFor(new ChainRequest { Id = target.UniqueId })}
                       };
        }
    }
}