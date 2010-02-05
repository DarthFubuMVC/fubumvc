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
        Chain
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

        public virtual void Append(BehaviorNode node)
        {
            BehaviorNode last = this.LastOrDefault() ?? this;
            last.Next = node;
        }

        public bool HasOutputBehavior()
        {
            return this.Any(x => x.Category == BehaviorCategory.Output);
        }

        public void InsertDirectlyAfter(BehaviorNode node)
        {
            BehaviorNode next = Next;
            Next = node;
            node.Next = next;
        }

        public void InsertDirectlyBefore(BehaviorNode newNode)
        {
            if (Previous != null) Previous.Next = newNode;
            newNode.Next = this;
        }
    }
}