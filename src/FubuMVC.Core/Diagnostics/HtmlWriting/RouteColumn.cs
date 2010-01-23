using System.Web.Routing;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using HtmlTags;
using System.Linq;

namespace FubuMVC.Core.Diagnostics.HtmlWriting
{
    public class RouteColumn : IColumn
    {
        public string Header()
        {
            return "Route";
        }

        public void WriteBody(BehaviorChain chain, HtmlTag cell)
        {
            string text = Text(chain);
            cell.Text(text);
        }

        public string Text(BehaviorChain chain)
        {
            if (chain.Route == null) return " -";

            var pattern = chain.Route.Pattern;
            if( pattern == string.Empty)
            {
                pattern = "(default)";
            }
            
            var httpMethodConstraint = chain.Route.Constraints.Where(kv => kv.Key == RouteConstraintPolicy.HTTP_METHOD_CONSTRAINT).Select(kv => kv.Value).FirstOrDefault() as HttpMethodConstraint;
            var methodList = httpMethodConstraint == null ? string.Empty : "[" + httpMethodConstraint.AllowedMethods.Join(",") + "] ";
            return methodList + pattern;
        }
    }
}