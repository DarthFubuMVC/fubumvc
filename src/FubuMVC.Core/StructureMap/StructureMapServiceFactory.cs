using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using StructureMap;
using StructureMap.Pipeline;

namespace FubuMVC.Core.StructureMap
{
    public class StructureMapServiceFactory : IServiceFactory, IDisposable
    {
        private readonly Stack<IContainer> _containers = new Stack<IContainer>();

        public StructureMapServiceFactory(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            _containers.Push(container);
        }

        public IContainer Container
        {
            get { return _containers.Peek(); }
        }

        public virtual IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            return new NestedStructureMapContainerBehavior(Container, arguments, behaviorId);
        }

        public T Build<T>(ServiceArguments arguments)
        {
            var explicitArguments = arguments.ToExplicitArgs();
            return Container.GetInstance<T>(explicitArguments);
        }

        public T Get<T>()
        {
            return Container.GetInstance<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return Container.GetAllInstances<T>();
        }


        public void Dispose()
        {
            _containers.Each(x => x.Dispose());
        }

        /// <summary>
        /// Creates a new StructureMap child container and makes that the new active container
        /// </summary>
        public void StartNewScope()
        {
            var child = Container.CreateChildContainer();
            _containers.Push(child);
        }

        /// <summary>
        /// Tears down any active child container and pops it out of the active container stack
        /// </summary>
        public void TeardownScope()
        {
            if (_containers.Count >= 2)
            {
                var child = _containers.Pop();
                child.Dispose();
            }
        }
    }
}