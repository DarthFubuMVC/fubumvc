using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting.Columns
{
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

            return chain.Route.AllowedHttpMethods.Any() ? chain.Route.AllowedHttpMethods.Join(",") : "All";
        }
    }
}