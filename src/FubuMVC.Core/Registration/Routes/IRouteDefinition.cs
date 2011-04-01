using System;
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

        Route ToRoute();
        void Append(string patternPart);
        
        void RemoveLastPatternPart();

        void AddRouteConstraint(string inputName, IRouteConstraint constraint);
        void Prepend(string prefix);

        void RootUrlAt(string baseUrl);

        string CreateTemplate(object input, Func<object, object>[] hash);

        IRouteInput Input { get; }

        void ApplyInputType(Type inputType);

        string CreateUrlFromInput(object input);
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