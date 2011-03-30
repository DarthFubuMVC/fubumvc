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
    public enum BehaviorCategory
    {
        Call,
        Output,
        Wrapper,
        Chain,
        Authorization
    }

    public class BehaviorSearch
    {
        public BehaviorSearch(Func<BehaviorNode, bool> matching)
        {
            Matching = matching;

            OnFound = n => { };
            OnMissing = () => { };
        }

        public Func<BehaviorNode, bool> Matching { get; set; }
        public Action<BehaviorNode> OnFound { get; set; }
        public Action OnMissing { get; set; }
    }

    public abstract class BehaviorNode : IEnumerable<BehaviorNode>
    {
        private readonly Guid _uniqueId = Guid.NewGuid();
        private BehaviorNode _next;
        public virtual Guid UniqueId { get { return _uniqueId; } }
        public abstract BehaviorCategory Category { get; }
        
        public BehaviorNode Next
        {
            get { return _next; }
            internal set
            {
                _next = value;
                if (value != null) value.Previous = this;
            }
        }

        public BehaviorNode Previous { get; internal set; }

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

        public BehaviorChain ParentChain()
        {
            if (Chain != null) return Chain;

            if (Previous == null) return null;

            return Previous.ParentChain();
        }

        public ObjectDef ToObjectDef()
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
                var dependency = new ConfiguredDependency
                {
                    Definition = Next.ToObjectDef(),
                    DependencyType = typeof (IActionBehavior)
                };

                objectDef.Dependencies.Add(dependency);
            }

            return objectDef;
        }

        protected abstract ObjectDef buildObjectDef();

        public bool HasOutputBehavior()
        {
            return this.Any(x => x.Category == BehaviorCategory.Output);
        }

        public void AddAfter(BehaviorNode node)
        {
            BehaviorNode next = Next;
            Next = node;
            node.Next = next;
        }

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

        public Wrapper WrapWith<T>() where T : IActionBehavior
        {
            return WrapWith(typeof (T));
        }

        public Wrapper WrapWith(Type behaviorType)
        {
            var wrapper = new Wrapper(behaviorType);
            AddBefore(wrapper);

            return wrapper;
        }

        public void AddToEnd(BehaviorNode node)
        {
            // Do not append any duplicates
            if (this.Contains(node)) return;

            BehaviorNode last = this.LastOrDefault() ?? this;
            last.Next = node;
        }

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