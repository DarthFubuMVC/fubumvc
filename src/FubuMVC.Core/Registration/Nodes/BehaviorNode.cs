using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuCore;

namespace FubuMVC.Core.Registration.Nodes
{
    public interface IContainerModel
    {
        /// <summary>
        /// Generates an ObjectDef object that creates an IoC agnostic
        /// configuration model of the real Behavior objects for this chain
        /// </summary>
        /// <returns></returns>
        ObjectDef ToObjectDef();
    }

    /// <summary>
    /// BehaviorNode models a single Behavior in the FubuMVC configuration model
    /// </summary>
    public abstract class BehaviorNode : IContainerModel, IEnumerable<BehaviorNode>
    {
        private readonly Guid _uniqueId = Guid.NewGuid();
        private BehaviorNode _next;
        public virtual Guid UniqueId { get { return _uniqueId; } }
        public abstract BehaviorCategory Category { get; }
        
        /// <summary>
        /// The next or "inner" BehaviorNode in this BehaviorChain
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
        /// The previous or "outer" BehaviorNode in this BehaviorChain
        /// </summary>
        public BehaviorNode Previous { get; internal set; }

        /// <summary>
        /// Carry out an action on any following or "inner" BehaviorNodes
        /// meeting a given criteria.
        /// </summary>
        /// <param name="search"></param>
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

        internal BehaviorChain Chain
        {
            get; set;
        }

        /// <summary>
        /// Retrieves the BehaviorChain that contains this
        /// BehaviorNode.  Does a recursive search up the chain
        /// </summary>
        /// <returns></returns>
        public BehaviorChain ParentChain()
        {
            if (Chain != null) return Chain;

            if (Previous == null) return null;

            return Previous.ParentChain();
        }

        /// <summary>
        /// Generates an ObjectDef object that creates an IoC agnostic
        /// configuration model of the real Behavior objects for this chain
        /// </summary>
        /// <returns></returns>
        ObjectDef IContainerModel.ToObjectDef()
        {
            ObjectDef objectDef = toObjectDef();
            objectDef.Name = UniqueId.ToString();

            return objectDef;
        }

        protected ObjectDef toObjectDef()
        {
            ObjectDef objectDef = buildObjectDef();
            if (Next != null)
            {
                var nextObjectDef = Next.As<IContainerModel>().ToObjectDef();
                objectDef.DependencyByType<IActionBehavior>(nextObjectDef);
            }

            return objectDef;
        }

        protected abstract ObjectDef buildObjectDef();

        /// <summary>
        /// Tests whether or not there are *any* output nodes
        /// after this BehaviorNode
        /// </summary>
        /// <returns></returns>
        public bool HasAnyOutputBehavior()
        {
            return this.Any(x => x.Category == BehaviorCategory.Output);
        }

        /// <summary>
        /// Inserts the BehaviorNode "node" immediately after this BehaviorNode.
        /// Any previously following BehaviorNodes will be attached after "node"
        /// </summary>
        /// <param name="node"></param>
        public void AddAfter(BehaviorNode node)
        {
            BehaviorNode next = Next;
            Next = node;
            node.Next = next;
        }

        /// <summary>
        /// Inserts the BehaviorNode "newNode" directly ahead of this BehaviorNode
        /// in the BehaviorChain.  All other ordering is preserved
        /// </summary>
        /// <param name="newNode"></param>
        public void AddBefore(BehaviorNode newNode)
        {
            Debug.WriteLine("Adding {0} before {1}".ToFormat(newNode.ToString(), this.ToString()));
            
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
        /// From innermost to outermost, iterates through the BehaviorNode's
        /// before this BehaviorNode in the BehaviorChain
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

        /// <summary>
        /// Shortcut to put a "wrapping" behavior immediately in front
        /// of this BehaviorNode.  Equivalent to AddBefore(Wrapper.For<T>())
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Wrapper WrapWith<T>() where T : IActionBehavior
        {
            return WrapWith(typeof (T));
        }


        /// <summary>
        /// Shortcut to put a "wrapping" behavior immediately in front of 
        /// this BehaviorNode.  Equivalent to AddBefore(new Wrapper(behaviorType))
        /// </summary>
        /// <param name="behaviorType"></param>
        /// <returns></returns>
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
        /// Adds a new BehaviorNode to the very end of this BehaviorChain
        /// </summary>
        /// <param name="node"></param>
        public void AddToEnd(BehaviorNode node)
        {
            // Do not append any duplicates
            if (this.Contains(node)) return;

            BehaviorNode last = this.LastOrDefault() ?? this;
            last.Next = node;
        }

        /// <summary>
        /// Removes only this BehaviorNode from the BehaviorChain.  Any following nodes
        /// would be attached to the previous BehaviorNode
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
        /// Swaps out this BehaviorNode for the given BehaviorNode
        /// </summary>
        /// <param name="newNode"></param>
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