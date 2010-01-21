using FubuMVC.Core.Registration.Nodes;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting
{
    public class ChainColumn : IColumn
    {
        public string Header()
        {
            return "Chain";
        }

        public void WriteBody(BehaviorChain chain, HtmlTag cell)
        {
            cell.Child(new LinkTag(Text(chain), "chain/" + chain.UniqueId).AddClass("chainId"));
        }

        public string Text(BehaviorChain chain)
        {
            return chain.UniqueId.ToString();
        }
    }
}