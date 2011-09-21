using System;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Rest.Conneg
{
    public class ConnegOutputNode : ConnegNode
    {
        public ConnegOutputNode(Type inputType) : base(inputType)
        {
        }

        protected override Type formatterActionType()
        {
            throw new NotImplementedException();
        }

        protected override ObjectDef buildObjectDef()
        {
            throw new NotImplementedException();
        }
    }
}