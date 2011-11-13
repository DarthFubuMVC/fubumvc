using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.Nodes
{
    public interface IContainerModel
    {
        /// <summary>
        ///   Generates an ObjectDef object that creates an IoC agnostic
        ///   configuration model of the real Behavior objects for this chain
        /// </summary>
        /// <param name = "diagnosticLevel"></param>
        /// <returns></returns>
        ObjectDef ToObjectDef(DiagnosticLevel diagnosticLevel);
    }

    /// <summary>
    ///   BehaviorNode models a single Behavior in the FubuMVC configuration model
    /// </summary>
    public abstract partial class BehaviorNode : IContainerModel, IEnumerable<BehaviorNode>
    {
        
        private BehaviorNode _next;


        public abstract BehaviorCategory Category { get; }

        /// <summary>
        ///   The next or "inner" BehaviorNode in this BehaviorChain
        /// </summary>
        public BehaviorNode Next
        {
            get { return _next; }
            internal set
            {
                _next = value;
                if (value != null) value.Previous = this;
            }
        }

        /// <summary>
        ///   The previous or "outer" BehaviorNode in this BehaviorChain
        /// </summary>
        public BehaviorNode Previous { get; internal set; }

        internal BehaviorChain Chain { get; set; }

        /// <summary>
        ///   From innermost to outermost, iterates through the BehaviorNodes
        ///   before this BehaviorNode in the BehaviorChain
        /// </summary>
        public IEnumerable<BehaviorNode> PreviousNodes
        {
            get
            {
                if (Previous == null) yield break;

                yield return Previous;

                foreach (var node in Previous.PreviousNodes)
                {
                    yield return node;
                }
            }
        }

        public virtual string Description
        {
            get { return GetType().GetFullName(); }
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<BehaviorNode> GetEnumerator()
        {
            if (Next != null)
            {
                yield return Next;

                foreach (BehaviorNode node in Next)
                {
                    yield return node;
                }
            }
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
        ///   Retrieves the BehaviorChain that contains this
        ///   BehaviorNode.  Does a recursive search up the chain
        /// </summary>
        /// <returns></returns>
        public BehaviorChain ParentChain()
        {
            if (Chain != null) return Chain;

            if (Previous == null) return null;

            return Previous.ParentChain();
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
        ///   Inserts the BehaviorNode "node" immediately after this BehaviorNode.
        ///   Any previously following BehaviorNodes will be attached after "node"
        /// </summary>
        /// <param name = "node"></param>
        public void AddAfter(BehaviorNode node)
        {
            var next = Next;
            Next = node;
            node.Next = next;
        }

        /// <summary>
        ///   Inserts the BehaviorNode "newNode" directly ahead of this BehaviorNode
        ///   in the BehaviorChain.  All other ordering is preserved
        /// </summary>
        /// <param name = "newNode"></param>
        public void AddBefore(BehaviorNode newNode)
        {            
            if (PreviousNodes.Contains(newNode)) return;

            newNode.Remove();

            if (Previous != null)
            {
                Previous.Next = newNode;
            }

            if (Previous == null && Chain != null)
            {
                Chain.Prepend(newNode);
            }

            newNode.Next = this;
        }

        /// <summary>
        ///   Shortcut to put a "wrapping" behavior immediately in front
        ///   of this BehaviorNode.  Equivalent to AddBefore(Wrapper.For<T>())
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
        /// Make the behavior *only* execute if the condition is met
        /// </summary>
        /// <param name="condition"></param>
        public void Condition(Func<bool> condition)
        {
            _conditionalDef = ConditionalObjectDef.For(condition);
        }

        /// <summary>
        /// Makes the behavior execute only if the condition against a service
        /// in the underlying IoC container is true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        public void ConditionByService<T>(Func<T, bool> condition)
        {
            _conditionalDef = ConditionalObjectDef.ForService(condition);
        }

        /// <summary>
        /// Makes the behavior execute only if the condition against a model
        /// object pulled from IFubuRequest is true
        /// </summary>
        public void ConditionByModel<T>(Func<T, bool> filter) where T : class
        {
            _conditionalDef = ConditionalObjectDef.ForModel(filter);
        }

        /// <summary>
        /// Makes the behavior execute only if the custom IConditional evaluates
        /// true
        /// </summary>
        public void Condition<T>() where T : IConditional
        {
            _conditionalDef = ConditionalObjectDef.For<T>();
        }


        /// <summary>
        ///   Adds a new BehaviorNode to the very end of this BehaviorChain
        /// </summary>
        public void AddToEnd(BehaviorNode node)
        {
            // Do not append any duplicates
            if (this.Contains(node)) return;

            var last = this.LastOrDefault() ?? this;
            last.Next = node;
        }

        /// <summary>
        ///   Removes only this BehaviorNode from the BehaviorChain.  Any following nodes
        ///   would be attached to the previous BehaviorNode
        /// </summary>
        public void Remove()
        {
            if (Next != null)
            {
                Next.Previous = Previous;
            }

            if (Previous == null && Chain != null && Next != null)
            {
                Chain.SetTop(Next);
            }

            if (Previous != null)
            {
                Previous.Next = Next;
            }

            Previous = null;
            Next = null;
        }

        /// <summary>
        ///   Swaps out this BehaviorNode for the given BehaviorNode
        /// </summary>
        public void ReplaceWith(BehaviorNode newNode)
        {
            newNode.Next = Next;

            if (Previous != null)
            {
                Previous.Next = newNode;
            }
            else if (Chain != null)
            {
                Chain.SetTop(newNode);
            }

            Previous = null;
            Next = null;
        }
    }
}