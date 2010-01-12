using System;
using FubuMVC.Core.Registration.Nodes;
using HtmlTags;

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
            return chain.Route == null ? " -" : chain.Route.Pattern;
        }
    }
}