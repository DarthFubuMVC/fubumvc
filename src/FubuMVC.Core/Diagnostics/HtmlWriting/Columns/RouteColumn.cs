using System;
using System.Web.Routing;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using HtmlTags;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Diagnostics.HtmlWriting.Columns
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
            if (shouldBeClickable(chain))
            {
                cell.Append(new LinkTag(text, chain.Route.Pattern.ToAbsoluteUrl()).AddClass("route-link"));
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

        private bool shouldBeClickable(BehaviorChain chain)
        {
            if (chain.IsPartialOnly) return false;

            if (chain == null || chain.Rank > 0) return false;
            if (chain.Route == null) return false;

            return chain.Route.RespondsToGet();

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