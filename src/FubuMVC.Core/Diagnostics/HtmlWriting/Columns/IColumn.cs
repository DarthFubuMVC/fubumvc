using FubuMVC.Core.Registration.Nodes;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting.Columns
{
    public interface IColumn
    {
        string Header();
        void WriteBody(BehaviorChain chain, HtmlTag row, HtmlTag cell);
        string Text(BehaviorChain chain);
    }
}