using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace FubuMVC.Core.Registration.Routes
{
    // TODO -- add Xml comments here, this is publicly facing
    public interface IRouteDefinition
    {
        string Pattern { get; }
        int Rank { get; }
        IRouteInput Input { get; }
        IList<string> AllowedHttpMethods { get; }

        Route ToRoute();

        void Append(string patternPart);

        void RemoveLastPatternPart();

        void Prepend(string prefix);

        void RootUrlAt(string baseUrl);

        void ApplyInputType(Type inputType);

        string CreateUrlFromInput(object input);
        void AddHttpMethodConstraint(string method);
        void RegisterRouteCustomization(Action<Route> action);

        SessionStateRequirement SessionStateRequirement { get; set; }

        bool RespondsToMethod(string method);
    }

    public static class RouteDefinitionExtensions
    {
        public static void ConstrainToHttpMethods(this IRouteDefinition routeDef, params string[] methods)
        {
            methods.Each(routeDef.AddHttpMethodConstraint);
        }
    }
}