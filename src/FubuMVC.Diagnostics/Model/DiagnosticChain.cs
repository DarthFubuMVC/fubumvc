using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Diagnostics.Chrome;

namespace FubuMVC.Diagnostics.Model
{
    public class DiagnosticChain : RoutedChain
    {
        public const string DiagnosticsUrl = "_fubu";
        private bool _isIndex;
        private ActionCall _call;

        public static IRouteDefinition BuildRoute(DiagnosticGroup group, ActionCall call)
        {
            if (call.Method.Name == "Index")
            {
                return new RouteDefinition("{0}/{1}".ToFormat(DiagnosticsUrl, group.Url).TrimEnd('/'));
            }

            var route = call.ToRouteDefinition();
            MethodToUrlBuilder.Alter(route, call);
            route.Prepend(@group.Url);
            route.Prepend(DiagnosticsUrl);

            return route;
        }

        // Going to need to change this to a factory, or at least do something to get the route
        public DiagnosticChain(DiagnosticGroup group, ActionCall call) : base(BuildRoute(group, call))
        {
            if (call.HasInput)
            {
                Route.ApplyInputType(call.InputType());
            }

            Group = group;

            if (call.Method.Name == "Index")
            {
                setupAsIndex(@group);
            }
            else
            {
                readTitleAndDescription(call);
            }

            _call = call;

            if (IsLink() || IsDetailsPage())
            {
                InsertFirst(new ChromeNode(typeof (DashboardChrome))
                {
                    Title = () => Title
                });
            }

            AddToEnd(call);

            
        }

        public bool IsDetailsPage()
        {
            return Route.RespondsToMethod("GET") && (Route.Input == null || Route.Input.RouteParameters.Any()) &&
                   !IsPartialOnly && GetRoutePattern().ToLower().Contains("/details/");

        }

        public bool IsIndex
        {
            get { return _isIndex; }
        }

        public DiagnosticGroup Group { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public static DiagnosticChain For<T>(DiagnosticGroup group, Expression<Action<T>> method)
        {
            ActionCall call = ActionCall.For(method);
            return new DiagnosticChain(group, call);
        }

        private void setupAsIndex(DiagnosticGroup @group)
        {
            
            Title = @group.Title;
            Description = @group.Description;
            _isIndex = true;
        }

        private void readTitleAndDescription(ActionCall call)
        {
            Title = Route.Pattern.Split('/').Last().Capitalize();
            Description = string.Empty;

            call.ForAttributes<DescriptionAttribute>(att => {
                if (att.Description.Contains(":"))
                {
                    string[] parts = att.Description.ToDelimitedArray(':');
                    Title = parts.First();
                    Description = parts.Last();
                }
                else
                {
                    Title = att.Description;
                }
            });
        }

        public bool IsLink()
        {
            return Route.RespondsToMethod("GET") && (Route.Input == null || !Route.Input.RouteParameters.Any()) &&
                   !_call.HasAttribute<FubuPartialAttribute>() && !_call.Method.Name.EndsWith("Partial");
        }
    }
}