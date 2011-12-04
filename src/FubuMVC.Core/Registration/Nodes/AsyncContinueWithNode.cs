using System;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Registration.Nodes
{
    public class AsyncContinueWithNode : Wrapper
    {
        public AsyncContinueWithNode(Type outputType) : base(typeof(AsyncContinueWithBehavior<>).MakeGenericType(outputType))
        {
        }

        public AsyncContinueWithNode() : base(typeof(AsyncContinueWithBehavior))
        {
        }
    }
}