using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Conneg
{
    public class ConnegNode : BehaviorNode
    {
        public Type InputType { get; set; }
        public Type OutputType { get; set; }

        public override BehaviorCategory Category
        {
            get { return OutputType == null ? BehaviorCategory.Process : BehaviorCategory.Output; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var objectDef = ObjectDef.ForType<ConnegBehavior>();
            objectDef.Dependency(typeof(IConnegInputHandler), buildInputHandlerDef());
            objectDef.Dependency(typeof(IConnegOutputHandler), buildOutputHandlerDef());

            return objectDef;
        }

        private ObjectDef buildOutputHandlerDef()
        {
            return OutputType == null 
                       ? ObjectDef.ForType<NulloConnegHandler>() 
                       : new ObjectDef(typeof(ConnegOutputHandler<>), OutputType);
        }

        private ObjectDef buildInputHandlerDef()
        {
            return InputType == null
                       ? ObjectDef.ForType<NulloConnegHandler>()
                       : new ObjectDef(typeof(ConnegInputHandler<>), InputType);
        }
    }
}