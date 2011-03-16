using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;

namespace FubuMVC.Diagnostics.Models.Routes
{
    public class RouteDataBuilder : IRouteDataBuilder
    {
        public IEnumerable<RouteDataModel> BuildRoutes(BehaviorGraph graph)
        {
            return graph
                .Behaviors
                .Select(chain =>
                            {
                                var pattern = "";
                                var inputModel = "";
                                var outputModel = "";

                                var httpMethodConstraint = chain
                                                            .Route
                                                            .Constraints
                                                            .Where(kv => kv.Key == RouteConstraintPolicy.HTTP_METHOD_CONSTRAINT)
                                                            .Select(kv => kv.Value)
                                                            .FirstOrDefault() as HttpMethodConstraint;

                                if (httpMethodConstraint != null)
                                {
                                    pattern = httpMethodConstraint.AllowedMethods.Join(",");
                                }

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
                                               Constraints = pattern,
                                               Action = chain.FirstCallDescription,
                                               InputModel = inputModel,
                                               OutputModel = outputModel
                                           };
                            })
                    .OrderBy(m => m.Route);
        }
    }
}