using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;

namespace FubuMVC.Diagnostics.Models.Routes
{
    public class RouteDataBuilder : IRouteDataBuilder
    {
        private readonly IUrlRegistry _urls;
        private readonly IHttpConstraintResolver _constraintResolver;

        public RouteDataBuilder(IUrlRegistry urls, IHttpConstraintResolver constraintResolver)
        {
            _urls = urls;
            _constraintResolver = constraintResolver;
        }

        public IEnumerable<RouteDataModel> BuildRoutes(BehaviorGraph graph)
        {
            return graph
                .Behaviors
                .Select(chain =>
                            {
                                
                                var inputModel = "";
                                var outputModel = "";
                                var constraints = _constraintResolver.Resolve(chain);

                                var call = chain.FirstCall();
                                if(call != null)
                                {
                                    var parameter = call.Method.GetParameters().FirstOrDefault();
                                    if(parameter != null)
                                    {
                                        inputModel = parameter.ParameterType.Name;
                                    }

                                    outputModel = call.Method.ReturnType.Name;
                                }

                                return new RouteDataModel
                                           {
                                               Id = chain.UniqueId.ToString(),
                                               Route = chain.RoutePattern,
                                               Constraints = constraints,
                                               Action = chain.FirstCallDescription,
                                               InputModel = inputModel,
                                               OutputModel = outputModel,
                                               ChainUrl = _urls.UrlFor(new ChainRequest { Id = chain.UniqueId })
                                           };
                            })
                    .OrderBy(m => m.Route);
        }
    }

    public interface IHttpConstraintResolver
    {
        string Resolve(BehaviorChain chain);
    }

    public class HttpConstraintResolver : IHttpConstraintResolver
    {
        public const string NoConstraints = "N/A";

        public string Resolve(BehaviorChain chain)
        {
            var constraint = chain
                                 .Route
                                 .Constraints
                                 .Where(kv => kv.Key == RouteConstraintPolicy.HTTP_METHOD_CONSTRAINT)
                                 .Select(kv => kv.Value)
                                 .FirstOrDefault() as HttpMethodConstraint;
            if(constraint == null)
            {
                return NoConstraints;
            }

            return constraint.AllowedMethods.Join(",");
        }
    }
}