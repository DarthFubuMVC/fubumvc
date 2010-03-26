using System;
using System.Reflection;
using System.Text.RegularExpressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteBuilder
    {
        public static RouteDefinition<T> Build<T>(string pattern)
        {
            var route = new RouteDefinition<T>(pattern);
            Type inputType = typeof (T);

            populateRoute(pattern, inputType, route);

            return route;
        }

        public static IRouteDefinition Build(Type inputType, string pattern)
        {
            Type routeType = typeof (RouteDefinition<>).MakeGenericType(inputType);
            var route = Activator.CreateInstance(routeType, pattern) as IRouteDefinition;

            populateRoute(pattern, inputType, route);

            return route;
        }

        private static void populateRoute(string pattern, Type inputType, IRouteDefinition route)
        {
            parse(pattern, (propName, defaultValue) =>
            {
                PropertyInfo property = inputType.GetProperty(propName);
                if (property == null)
                    throw new FubuException(1002, "Url pattern \"{0}\" refers to non-existent property {1} on {2}.",
                                            pattern, propName, inputType.FullName);
                var input = new RouteInput(new SingleProperty(property))
                {
                    DefaultValue = defaultValue
                };
                route.AddRouteInput(input, false);
            });
        }


        private static void parse(string template, Action<string, string> callback)
        {
            const string propertyFindingPattern =
                @"
\{              # start variable
(?<varname>\w+) # capture 1 or more word characters as the variable name
(:              # optional section beginning with a colon
(?<default>\w+) # capture 1 or more word characters as the default value
)?              # end optional section
\}              # end variable";

            foreach (
                Match match in Regex.Matches(template, propertyFindingPattern, RegexOptions.IgnorePatternWhitespace))
            {
                string defaultValue = match.Groups["default"].Success ? match.Groups["default"].Value : null;
                callback(match.Groups["varname"].Value, defaultValue);
            }
        }
    }
}