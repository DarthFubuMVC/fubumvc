using FubuMVC.Core.Registration.Nodes;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting.Columns
{
    public class ChainColumn : IColumn
    {
        public string Header()
        {
            return "Chain";
        }

        public void WriteBody(BehaviorChain chain, HtmlTag row, HtmlTag cell)
        {
            cell.Append(new LinkTag("Behaviors", "chain/" + chain.UniqueId).AddClass("chainId"));
        }

        public string Text(BehaviorChain chain)
        {
            return chain.UniqueId.ToString();
        }
    }
}