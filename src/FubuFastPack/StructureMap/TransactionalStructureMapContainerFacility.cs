using System;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.StructureMap;
using StructureMap;

namespace FubuFastPack.StructureMap
{
    public class TransactionalStructureMapContainerFacility : StructureMapContainerFacility
    {
        private readonly IContainer _container;

        public TransactionalStructureMapContainerFacility(IContainer container) : base(container)
        {
            _container = container;
        }

        public override IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            return new TransactionalContainerBehavior(_container, arguments, behaviorId);
        }
    }

    
    public class TransactionalContainerBehavior : IActionBehavior
    {
        private readonly ServiceArguments _arguments;
        private readonly Guid _behaviorId;
        private readonly IContainer _container;

        public TransactionalContainerBehavior(IContainer container, ServiceArguments arguments, Guid behaviorId)
        {
            _container = container;
            _arguments = arguments;
            _behaviorId = behaviorId;
        }

        public void Invoke()
        {
            _container.ExecuteInTransaction<IContainer>(invokeRequestedBehavior);
        }

        public void InvokePartial()
        {
            // Just go straight to the inner behavior here.  Assuming that the transaction & principal
            // are already set up
            invokeRequestedBehavior(_container);
        }

        private void invokeRequestedBehavior(IContainer c)
        {
            var behavior = c.GetInstance<IActionBehavior>(_arguments.ToExplicitArgs(), _behaviorId.ToString());
            behavior.Invoke();
        }
    }
}