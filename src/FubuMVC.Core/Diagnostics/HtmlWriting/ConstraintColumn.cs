namespace FubuMVC.Core.Diagnostics.HtmlWriting
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Routing;
    using HtmlTags;
    using Registration.Conventions;
    using Registration.Nodes;

    public class ConstraintColumn : IColumn
    {
        public string Header()
        {
            return "Constraints";
        }

        public void WriteBody(BehaviorChain chain, HtmlTag row, HtmlTag cell)
        {
            var text = Text(chain);
            cell.Text(text);
        }

        public string Text(BehaviorChain chain)
        {
            if (chain.Route == null) return " -";

            var httpMethodConstraint = chain.Route.Constraints.Where(kv => kv.Key == RouteConstraintPolicy.HTTP_METHOD_CONSTRAINT).Select(kv => kv.Value).FirstOrDefault() as HttpMethodConstraint;
            var pattern = "";
            
            if (httpMethodConstraint != null)
                pattern = httpMethodConstraint.AllowedMethods.Join(",");

            return pattern;
        }
    }
}