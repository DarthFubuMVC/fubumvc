using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public abstract class ModifyChainAttribute : Attribute
    {
        public abstract void Alter(ActionCallBase call);
    }
}