using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuCore.Util;

namespace FubuMVC.Core.Registration.Routes
{
    // TODO -- add Xml comments here, this is publicly facing
    public interface IRouteDefinition
    {
        string Pattern { get; }
        int Rank { get; }
        Indexer<string, IRouteConstraint> Constraints { get; }
        IRouteInput Input { get; }
        IList<string> AllowedHttpMethods { get; }

        Route ToRoute();

        void Append(string patternPart);

        void RemoveLastPatternPart();

        void Prepend(string prefix);

        void RootUrlAt(string baseUrl);

        string CreateTemplate(object input, Func<object, object>[] hash);

        void ApplyInputType(Type inputType);

        string CreateUrlFromInput(object input);
        void AddHttpMethodConstraint(string method);
        void RegisterRouteCustomization(Action<Route> action);

        SessionStateRequirement SessionStateRequirement { get; set; }

        bool RespondsToMethod(string method);

        /// <summary>
        /// Provides metadata for the usage of this route definition. 
        /// This is particularly useful when creating route aliases.
        /// </summary>
        string Category { get; set; }
    }

    public static class RouteDefinitionExtensions
    {
        public static void ConstrainToHttpMethods(this IRouteDefinition routeDef, params string[] methods)
        {
            methods.Each(routeDef.AddHttpMethodConstraint);
        }
    }
}