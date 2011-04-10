using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting.Columns
{
    public class InputModelColumn : IColumn
    {
        public string Header()
        {
            return "Input Model";
        }

        public void WriteBody(BehaviorChain chain, HtmlTag row, HtmlTag cell)
        {
            Type inputType = chain.InputType();
            
            if (inputType == null)
            {
                cell.Text(" -");
            }
            else
            {
                cell.Text(inputType.Name).Title(inputType.AssemblyQualifiedName);
            }
        }

        public string Text(BehaviorChain chain)
        {
            Type inputType = chain.InputType();
            return inputType == null ? " -" : "{0} ({1})".ToFormat(inputType.Name, inputType.FullName);
        }
    }
}