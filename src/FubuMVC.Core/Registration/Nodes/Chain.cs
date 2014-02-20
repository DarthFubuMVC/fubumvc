using System;
using System.Collections;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.Registration.Nodes
{
    public interface INode<T>
    {
        void AddAfter(T node);
        void AddBefore(T node);

    }

    public abstract class Chain<T, TChain> : INode<T>, IEnumerable<T> 
        where T : Node<T, TChain> 
        where TChain : Chain<T, TChain>
    {
        private T _top;


        /// <summary>
        ///   Adds a new Node to the very end of this behavior chain
        /// </summary>
        /// <param name = "node"></param>
        public void AddToEnd(T node)
        {
            if (Top == null)
            {
                SetTop(node);
                return;
            }

            Top.AddToEnd(node);
        }

        /// <summary>
        ///   Adds a new Node of type T to the very end of this
        ///   behavior chain
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <returns></returns>
        public TNode AddToEnd<TNode>() where TNode : T, new()
        {
            var node = new TNode();
            AddToEnd(node);
            return node;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (Top == null) yield break;

            yield return Top;

            foreach (var node in Top)
            {
                yield return node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        internal void SetTop(T node)
        {
            if (node == null)
            {
                _top = null;
            }
            else
            {
                node.Previous = null;

                if (_top != null)
                {
                    _top.Chain = null;
                }

                _top = node;
                node.Chain = this.As<TChain>();
            }
        }

        public void InsertFirst(T node)
        {
            var previousTop = _top;

            SetTop(node);

            if (previousTop != null)
            {
                _top.Next = previousTop;
            }
        }

        /// <summary>
        /// The outermost Node in the chain
        /// </summary>
        public T Top
        {
            get { return _top; }
        }

        /// <summary>
        /// Sets the specified Node as the outermost node
        /// in this chain
        /// </summary>
        /// <param name="node"></param>
        public void Prepend(T node)
        {
            var next = Top;
            SetTop(node);

            if (next != null)
            {
                Top.Next = next;
            }
        }


        void INode<T>.AddAfter(T node)
        {
            AddToEnd(node);
        }

        void INode<T>.AddBefore(T node)
        {
            throw new NotSupportedException();
        }
    }
}