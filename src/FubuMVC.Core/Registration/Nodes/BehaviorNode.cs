using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;

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

    public abstract class BehaviorNode : IEnumerable<BehaviorNode>
    {
        private readonly Guid _uniqueId = Guid.NewGuid();
        private BehaviorNode _next;
        public virtual Guid UniqueId { get { return _uniqueId; } }
        public abstract BehaviorCategory Category { get; }

        public BehaviorNode Next
        {
            get { return _next; }
            protected set
            {
                _next = value;
                if (value != null) value.Previous = this;
            }
        }

        public BehaviorNode Previous { get; protected set; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual IEnumerator<BehaviorNode> GetEnumerator()
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

        public BehaviorChain ParentChain()
        {
            if (this is BehaviorChain) return (BehaviorChain) this;

            if (Previous == null) return null;

            if (Previous is BehaviorChain) return (BehaviorChain) Previous;

            return Previous.ParentChain();
        }

        public virtual ObjectDef ToObjectDef()
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
            if (Previous != null) Previous.Next = newNode;
            newNode.Next = this;
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

        public virtual void AddToEnd(BehaviorNode node)
        {
            BehaviorNode last = this.LastOrDefault() ?? this;
            last.Next = node;
        }

        public virtual void Remove()
        {
            if (Previous == null)
            {
                Next.Previous = null;
            }
            else
            {
                Previous.Next = Next;
            }
            Previous = null;
            Next = null;
        }

        public virtual void ReplaceWith(BehaviorNode newNode)
        {
            if (Previous != null)
            {
                Previous.Next = newNode;
            }
            newNode.Next = Next;
            Previous = null;
            Next = null;
        }
    }
}