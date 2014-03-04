using System;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core
{
    /// <summary>
    /// Marks this endpoint as the creator of the designated type
    /// so that this endpoint will resolve as IUrlRegistry.UrlForNew(type)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UrlForNewAttribute : ModifyChainAttribute
    {
        private readonly Type _type;

        public UrlForNewAttribute(Type type)
        {
            _type = type;
        }

        public Type Type
        {
            get { return _type; }
        }

        public override void Alter(ActionCall call)
        {
            call.ParentChain().As<RoutedChain>().UrlCategory.Creates.Add(Type);
        }
    }
}