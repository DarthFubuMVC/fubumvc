using System;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Registration.Nodes
{
    public class ContinuationNode : Wrapper
    {
        public ContinuationNode() : base(typeof(ContinuationHandler))
        {
        }
    }
}