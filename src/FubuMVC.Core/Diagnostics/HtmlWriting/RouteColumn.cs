using System;
using System.Web.Routing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using HtmlTags;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Diagnostics.HtmlWriting
{
    public class RouteColumn : IColumn
    {
        public string Header()
        {
            return "Route";
        }

        public void WriteBody(BehaviorChain chain, HtmlTag row, HtmlTag cell)
        {
            var text = Text(chain);
            if (shouldBeClickable(chain.Route))
            {
                cell.Child(new LinkTag(text, chain.Route.Pattern.ToAbsoluteUrl()).AddClass("route-link"));
            }
            else
            {
                cell.Text(text);
            }
            if (text.StartsWith(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT))
            {
                row.AddClass(BehaviorGraphWriter.FUBU_INTERNAL_CLASS);
            }
        }

        private bool shouldBeClickable(IRouteDefinition routeDefinition)
        {
            if (routeDefinition == null || routeDefinition.Rank > 0) return false;
            if (routeDefinition is NulloRouteDefinition) return false;
            var httpConstraint = routeDefinition.Constraints.Select(c => c.Value).OfType<HttpMethodConstraint>().FirstOrDefault();
            if (httpConstraint != null && !httpConstraint.AllowedMethods.Any(m => m.Equals("GET", StringComparison.OrdinalIgnoreCase))) return false;
            return true;
        }

        public string Text(BehaviorChain chain)
        {
            if (chain.Route == null) return " -";

            var pattern = chain.Route.Pattern;
            if( pattern == string.Empty)
            {
                pattern = "(default)";
            }
            
            //var httpMethodConstraint = chain.Route.Constraints.Where(kv => kv.Key == RouteConstraintPolicy.HTTP_METHOD_CONSTRAINT).Select(kv => kv.Value).FirstOrDefault() as HttpMethodConstraint;
            //var methodList = httpMethodConstraint == null ? string.Empty : "[" + httpMethodConstraint.AllowedMethods.Join(",") + "] ";
            //return methodList + pattern;

            return pattern;
        }
    }
}