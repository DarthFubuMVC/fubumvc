using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Instrumentation;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Registration.Nodes
{
    /// <summary>
    ///   BehaviorNode models a single Behavior in the FubuMVC configuration model
    /// </summary>
    public abstract class BehaviorNode : Node<BehaviorNode, BehaviorChain>, IContainerModel, ISubject
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
            get { return buildInstance().PluggedType; }
        }

        public abstract BehaviorCategory Category { get; }

        public virtual string Description
        {
            get { return GetType().GetFullName(); }
        }

        /// <summary>
        ///   Carry out an action on any following or "inner" BehaviorNodes
        ///   meeting a given criteria.
        /// </summary>
        /// <param name = "search"></param>
        public void ForFollowingBehavior(BehaviorSearch search)
        {
            var follower = this.FirstOrDefault(search.Matching);
            if (follower != null)
            {
                search.OnFound(follower);
            }
            else
            {
                search.OnMissing();
            }
        }


        /// <summary>
        ///   Tests whether or not there are *any* output nodes
        ///   after this BehaviorNode
        /// </summary>
        /// <returns></returns>
        public bool HasAnyOutputBehavior()
        {
            return this.Any(x => x.Category == BehaviorCategory.Output);
        }

        /// <summary>
        ///   Shortcut to put a "wrapping" behavior immediately in front
        ///   of this BehaviorNode.  Equivalent to AddBefore(Wrapper.For[T]())
        /// </summary>
        public Wrapper WrapWith<T>() where T : IActionBehavior
        {
            return WrapWith(typeof (T));
        }


        /// <summary>
        ///   Shortcut to put a "wrapping" behavior immediately in front of 
        ///   this BehaviorNode.  Equivalent to AddBefore(new Wrapper(behaviorType))
        /// </summary>
        public Wrapper WrapWith(Type behaviorType, params Type[] parameterTypes)
        {
            if (behaviorType.IsOpenGeneric())
            {
                behaviorType = behaviorType.MakeGenericType(parameterTypes);
            }

            if (!behaviorType.CanBeCastTo<IActionBehavior>())
            {
                throw new FubuException(1010, "Type {0} does not implement IActionBehavior and cannot be used here");
            }

            var wrapper = new Wrapper(behaviorType);
            AddBefore(wrapper);

            return wrapper;
        }

        string ISubject.Title()
        {
            return Description;
        }

        Guid ISubject.Id
        {
            get { return UniqueId; }
        }
    }
}