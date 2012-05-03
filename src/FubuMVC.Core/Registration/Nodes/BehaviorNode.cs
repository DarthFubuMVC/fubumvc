using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.Diagnostics;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Registration.Nodes
{
    /// <summary>
    ///   BehaviorNode models a single Behavior in the FubuMVC configuration model
    /// </summary>
    public abstract partial class BehaviorNode : Node<BehaviorNode, BehaviorChain>, IContainerModel
    {
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
        [MarkedForTermination]
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


        /// <summary>
        ///   Make the behavior *only* execute if the condition is met
        /// </summary>
        /// <param name = "condition"></param>
        /// <param name="description"></param>
        public void Condition(Func<bool> condition, string description = "Anonymous")
        {
            Trace(new ConditionAdded(description));
            _conditionalDef = ConditionalObjectDef.For(condition);
        }

        /// <summary>
        ///   Makes the behavior execute only if the condition against a service
        ///   in the underlying IoC container is true
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "condition"></param>
        public void ConditionByService<T>(Func<T, bool> condition)
        {
            var description = "By Service:  Func<{0}, bool>".ToFormat(typeof (T).Name);
            Trace(new ConditionAdded(description));
            _conditionalDef = ConditionalObjectDef.ForService(condition);
        }

        /// <summary>
        ///   Makes the behavior execute only if the condition against a model
        ///   object pulled from IFubuRequest is true
        /// </summary>
        public void ConditionByModel<T>(Func<T, bool> filter) where T : class
        {
            var description = "By Model:  Func<{0}, bool>".ToFormat(typeof(T).Name);
            Trace(new ConditionAdded(description));
            _conditionalDef = ConditionalObjectDef.ForModel(filter);
        }

        /// <summary>
        ///   Makes the behavior execute only if the custom IConditional evaluates
        ///   true
        /// </summary>
        public void Condition<T>() where T : IConditional
        {
            Trace(new ConditionAdded(typeof (T)));
            _conditionalDef = ConditionalObjectDef.For<T>();
        }
    }
}