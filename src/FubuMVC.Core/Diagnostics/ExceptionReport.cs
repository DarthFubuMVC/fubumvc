using System;

namespace FubuMVC.Core.Diagnostics
{
    public class ExceptionReport : IBehaviorDetails
    {
        public string Text { get; set; }

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            visitor.Exception(this);
        }
    }
}