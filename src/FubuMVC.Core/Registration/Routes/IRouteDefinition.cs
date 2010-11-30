using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;
using FubuMVC.Core.Registration.Conventions;

namespace FubuMVC.Core.Registration.Routes
{
    public interface IRouteDefinition
    {
        string Pattern { get; }
        int Rank { get; }
        IEnumerable<KeyValuePair<string, object>> Constraints { get; }
        string CreateUrl(object input);
        Route ToRoute();
        void Append(string patternPart);
        void AddRouteInput(RouteInput input, bool appendToUrl);
        void RemoveLastPatternPart();
        void AddQueryInput(PropertyInfo property);
        void AddRouteConstraint(string inputName, IRouteConstraint constraint);
        void Prepend(string prefix);

        void RootUrlAt(string baseUrl);
    }

    public static class RouteDefinitionExtensions
    {
        public static void ConstrainToHttpMethods(this IRouteDefinition routeDef, params string[] methods)
        {
            if(methods.Length == 0)
            {
                return;
            }

            routeDef.AddRouteConstraint(RouteConstraintPolicy.HTTP_METHOD_CONSTRAINT, new HttpMethodConstraint(methods));
        }
    }
}