using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration
{
    public class EndpointActionSource : IActionSource
    {
        public IEnumerable<ActionCall> FindActions(TypePool types)
        {
            return types.TypesMatching(t => t.Name.EndsWith("Endpoint") || t.Name.EndsWith("Endpoints"))
                .SelectMany(type =>
                {
                    var methods = type.GetMethods().Where(ActionSource.IsCandidate);
                    return methods.Select(m => new ActionCall(type, m));
                });
        }
    }
}