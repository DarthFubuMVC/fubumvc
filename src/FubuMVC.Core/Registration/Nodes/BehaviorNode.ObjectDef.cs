using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Registration.Nodes
{
    public abstract partial class BehaviorNode
    {

        protected BehaviorNode()
        {
            UniqueId = Guid.NewGuid();
        }

        public virtual Guid UniqueId { get; protected set; }

        Instance IContainerModel.ToInstance()
        {
            var instance = toInstance();
            instance.Name = UniqueId.ToString();

            return instance.As<Instance>();
        }

        protected IConfiguredInstance toInstance()
        {
            var instance = buildInstance();

            if (Next != null)
            {
                var next = Next.As<IContainerModel>().ToInstance();
                instance.Dependencies.Add(typeof(IActionBehavior), next);
            }

            return instance;
        }

        protected abstract IConfiguredInstance buildInstance();

        public Type BehaviorType
        {
            get
            {
                var objectDef = buildObjectDef();
                return objectDef.Type ?? objectDef.Value.GetType();
            }
        }

        ObjectDef IContainerModel.ToObjectDef()
        {
            var objectDef = toObjectDef();
            objectDef.Name = UniqueId.ToString();

            return objectDef;
        }

        protected ObjectDef toObjectDef()
        {
            var objectDef = buildObjectDef();

            if (Next != null)
            {
                attachNextBehavior(objectDef);
            }

            return objectDef;
        }

        private void attachNextBehavior(ObjectDef objectDef)
        {
            var nextObjectDef = Next.As<IContainerModel>().ToObjectDef();
            objectDef.DependencyByType<IActionBehavior>(nextObjectDef);
        }

        protected abstract ObjectDef buildObjectDef();
    }
}