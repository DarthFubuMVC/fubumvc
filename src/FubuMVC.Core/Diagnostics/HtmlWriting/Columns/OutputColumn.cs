using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting.Columns
{
    public class OutputColumn : IColumn
    {
        public string Header()
        {
            return "Output(s)";
        }

        public void WriteBody(BehaviorChain chain, HtmlTag row, HtmlTag cell)
        {
            cell.Text(Text(chain));
        }

        public string Text(BehaviorChain chain)
        {
            return chain.Outputs.Count() == 0 
                       ? " -" 
                       : chain.Outputs.Select(x => x.Description).Join(", ");
        }
    }
}