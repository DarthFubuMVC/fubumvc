using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Registration.Nodes
{
    public abstract class Node<T, TChain> : INode<T>, IEnumerable<T> 
        where T : Node<T, TChain> 
        where TChain : Chain<T, TChain>
    {
        private T _next;

        /// <summary>
        ///   Retrieves the Chain that contains this
        ///   Node.  Does a recursive search up the chain
        /// </summary>
        /// <returns></returns>
        public TChain ParentChain()
        {
            if (Chain != null) return Chain;

            if (Previous == null) return null;

            return Previous.ParentChain();
        }

        internal TChain Chain { get; set; }

        /// <summary>
        ///   The next or "inner" Node
        /// </summary>
        public T Next
        {
            get { return _next; }
            internal set
            {
                if (ReferenceEquals(value, this))
                {
                    throw new InvalidOperationException("Cannot set Node.Next to itself unless you just like StackOverflowExceptions");
                }

                _next = value;
                
                if (value != null) value.Previous = this.As<T>();
            }
        }

        /// <summary>
        ///   The previous or "outer" Node in this chain
        /// </summary>
        public T Previous { get; internal set; }

        /// <summary>
        ///   From innermost to outermost, iterates through the Nodes
        ///   before this Node in the Chain
        /// </summary>
        public IEnumerable<T> PreviousNodes
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
        ///   Inserts the Node "node" immediately after this Node.
        ///   Any previously following Nodes will be attached after "node"
        /// </summary>
        /// <param name = "node"></param>
        public void AddAfter(T node)
        {
            var next = Next;
            Next = node;
            node.Next = next;
        }

        /// <summary>
        ///   Inserts the Node "newNode" directly ahead of this BehaviorNode
        ///   in the Chain.  All other ordering is preserved
        /// </summary>
        /// <param name = "newNode"></param>
        public void AddBefore(T newNode)
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

            newNode.Next = this.As<T>();
        }

        /// <summary>
        ///   Adds a new Node to the very end of this BehaviorChain
        /// </summary>
        public void AddToEnd(T node)
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

            if (Previous == null && Chain != null)
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
        public void ReplaceWith(T newNode)
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (Next != null)
            {
                yield return Next;

                foreach (T node in Next)
                {
                    yield return node;
                }
            }
        }

        /// <summary>
        /// Moves this node to the very beginning of the chain
        /// </summary>
        public void MoveToFront()
        {
            var chain = ParentChain();
            if (chain == null)
            {
                return;
            }

            Remove();
            chain.InsertFirst((T) this);
        }
    }
}