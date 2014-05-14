using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteBuilder
    {
        const string propertyFindingPattern =
            @"
\{              # start variable
\*?             # optional greedy token
(?<varname>\w+) # capture 1 or more word characters as the variable name
(:              # optional section beginning with a colon
(?<default>\w+) # capture 1 or more word characters as the default value
)?              # end optional section
\}              # end variable";

        public static RouteDefinition Build<T>(string pattern)
        {
            var parent = new RouteDefinition(pattern);
            Type inputType = typeof (T);

            parent.ApplyInputType(inputType);

            return parent;
        }

        public static IRouteDefinition Build(Type inputType, string pattern)
        {
            var parent = new RouteDefinition(pattern);
            Type routeType = typeof (RouteInput<>).MakeGenericType(inputType);
            var input = Activator.CreateInstance(routeType, parent) as IRouteInput;

            populateRoute(pattern, inputType, input);
            parent.Input = input;

            return parent;
        }

        private static void populateRoute(string pattern, Type inputType, IRouteInput input)
        {
            parse(pattern, (propName, defaultValue) =>
            {
                PropertyInfo property = inputType.GetProperty(propName);
                if (property == null)
                    throw new FubuException(1002, "Url pattern \"{0}\" refers to non-existent property {1} on {2}.",
                                            pattern, propName, inputType.FullName);
                var parameter = new RouteParameter(new SingleProperty(property))
                {
                    DefaultValue = defaultValue
                };

                input.AddRouteInput(parameter, false);
            });
            
            // Populate the remaining QueryString parameters
            PopulateQuerystringParameters(inputType, input);
        }

        public static void PopulateQuerystringParameters(Type inputType, IRouteInput input)
        {
            var props = inputType.GetProperties();
            foreach (var propertyInfo in props)
            {
                PropertyInfo info = propertyInfo;
                if (propertyInfo.HasAttribute<QueryStringAttribute>()
                    && !input.RouteParameters.Any(p => p.Name == info.Name))
                {
                    input.AddQueryInput(propertyInfo);
                }
            }
        }


        private static void parse(string template, Action<string, string> callback)
        {


            foreach (
                Match match in Regex.Matches(template, propertyFindingPattern, RegexOptions.IgnorePatternWhitespace))
            {
                string defaultValue = match.Groups["default"].Success ? match.Groups["default"].Value : null;
                callback(match.Groups["varname"].Value, defaultValue);
            }
        }

        public static int PatternRank(string pattern)
        {
            return Regex.Matches(pattern, propertyFindingPattern, RegexOptions.IgnorePatternWhitespace).Count;
        }
    }
}