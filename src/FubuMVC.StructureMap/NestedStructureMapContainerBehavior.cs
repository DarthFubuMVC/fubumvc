using System;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using StructureMap;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap
{
    public class NestedStructureMapContainerBehavior : IActionBehavior
    {
        private readonly ServiceArguments _arguments;
        private readonly Guid _behaviorId;
        private readonly IContainer _container;

        public NestedStructureMapContainerBehavior(IContainer container, ServiceArguments arguments, Guid behaviorId)
        {
            _container = container;
            _arguments = arguments;
            _behaviorId = behaviorId;
        }


        public void Invoke()
        {
            using (IContainer nested = _container.GetNestedContainer())
            {
                nested.Configure(x => _arguments.EachService((type, value) => x.For(type).Use(value)));
                var behavior = nested.GetInstance<IActionBehavior>(_behaviorId.ToString());
                behavior.Invoke();
            }
        }

        public void InvokePartial()
        {
            // This should never be called
            throw new NotSupportedException();
        }
    }

    public static class ServiceArgumentsExtensions
    {
        public static ExplicitArguments ToExplicitArgs(this ServiceArguments arguments)
        {
            var explicits = new ExplicitArguments();
            arguments.EachService(explicits.Set);

            return explicits;
        }
    }
}