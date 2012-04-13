using System.Collections;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.Registration.Nodes
{
    public abstract class Chain<T, TChain> : TracedNode, IEnumerable<T> 
        where T : Node<T, TChain> 
        where TChain : Chain<T, TChain>
    {
        private T _top;


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
            node.Previous = null;

            if (_top != null)
            {
                _top.Chain = null;
            }

            _top = node;
            node.Chain = this.As<TChain>();
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


    }
}