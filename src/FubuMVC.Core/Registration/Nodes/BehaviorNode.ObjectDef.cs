using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
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
            get { return toInstance().PluggedType; }
        }

    }
}