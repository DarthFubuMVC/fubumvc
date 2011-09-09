using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class HandlersUrlPolicy : IUrlPolicy
    {
        public const string HANDLER = "Handler";
        public const string METHOD = "Execute";
        public static readonly Regex HandlerExpression = new Regex("_[hH]andler", RegexOptions.Compiled);

        private readonly IEnumerable<Type> _markerTypes;

        public HandlersUrlPolicy(params Type[] markerTypes)
        {
            _markerTypes = markerTypes;
        }

        public virtual bool Matches(ActionCall call, IConfigurationObserver log)
        {
            if (!IsHandlerCall(call))
            {
                return false;
            }

            log.RecordCallStatus(call, "Matched on {0}".ToFormat(GetType().Name));
            return true;
        }

        public virtual IRouteDefinition Build(ActionCall call)
        {
            var routeDefinition = call.ToRouteDefinition();
            var strippedNamespace = stripNamespace(call);
            
            visit(routeDefinition);
            
            if (strippedNamespace != call.HandlerType.Namespace)
            {
                if (!strippedNamespace.Contains("."))
                {
                    routeDefinition.Append(breakUpCamelCaseWithHypen(strippedNamespace));
                }
                else
                {
                    var patternParts = strippedNamespace.Split(new[] { "." }, StringSplitOptions.None);
                    foreach (var patternPart in patternParts)
                    {
                        routeDefinition.Append(breakUpCamelCaseWithHypen(patternPart.Trim()));
                    }
                }
            }

            var handlerName = call.HandlerType.Name;
            var match = HandlerExpression.Match(handlerName);
            if (match.Success && MethodToUrlBuilder.Matches(handlerName))
            {
                // We're forcing handlers to end with "_handler" in this case
                handlerName = handlerName.Substring(0, match.Index);
                var properties = call.HasInput
                                 ? new TypeDescriptorCache().GetPropertiesFor(call.InputType()).Keys
                                 : new string[0];

                MethodToUrlBuilder.Alter(routeDefinition, handlerName, properties, text => { });

                if (call.HasInput)
                {
                    routeDefinition.ApplyInputType(call.InputType());
                }
            }
            else
            {
                // Otherwise we're expecting something like "GetHandler"
                var httpMethod = call.HandlerType.Name.Replace(HANDLER, string.Empty);
                routeDefinition.ConstrainToHttpMethods(httpMethod.ToUpper());
            }

            return routeDefinition;
        }

        protected virtual void visit(IRouteDefinition routeDefinition)
        {
            // no-op
        }

        private string stripNamespace(ActionCall call)
        {
            var strippedNamespace = "";

            _markerTypes
                .Each(marker =>
                          {
                              strippedNamespace = call
                                  .HandlerType
                                  .Namespace
                                  .Replace(marker.Namespace + ".", string.Empty);
                          });

            return strippedNamespace;
        }

        private static string breakUpCamelCaseWithHypen(string input)
        {
            var routeBuilder = new StringBuilder();
            for (int i = 0; i < input.Length; ++i)
            {
                if (i != 0 && char.IsUpper(input[i]))
                {
                    routeBuilder.Append("-");
                }

                routeBuilder.Append(input[i]);
            }

            return routeBuilder
                .ToString()
                .ToLower();
        }

        public static bool IsHandlerCall(ActionCall call)
        {
            var isHandler = call.HandlerType.Name.ToLower().EndsWith(HANDLER.ToLower()) || HandlerExpression.IsMatch(call.HandlerType.Name);
            return isHandler && !call.Method.HasAttribute<UrlPatternAttribute>();
        }
    }
}