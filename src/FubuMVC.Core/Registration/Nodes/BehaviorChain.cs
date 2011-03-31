using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Registration.Nodes
{
    /// <summary>
    /// BehaviorChain is a configuration model for a single endpoint in a 
    /// FubuMVC system.  Models route information, the behaviors, and 
    /// authorization rules
    ///   system
    /// </summary>
    public class BehaviorChain : IEnumerable<BehaviorNode>
    {
        private BehaviorNode _top;

        public BehaviorChain()
        {
            Authorization = new AuthorizationNode();
            UrlCategory = new UrlCategory();
        }

        public Guid UniqueId
        {
            get { return Top == null ? Guid.Empty : Top.UniqueId; }
        }

        /// <summary>
        ///   Categorizes this BehaviorChain for the IUrlRegistry and 
        ///   IEndpointService UrlFor(***, category) methods
        /// </summary>
        public UrlCategory UrlCategory { get; private set; }

        /// <summary>
        /// The outermost BehaviorNode in the chain
        /// </summary>
        public BehaviorNode Top
        {
            get { return _top; }
        }

        /// <summary>
        /// Marks what package or FubuRegistry created this BehaviorChain
        /// for the sake of diagnostics
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// All the ActionCall nodes in this chain
        /// </summary>
        public IEnumerable<ActionCall> Calls
        {
            get { return this.OfType<ActionCall>(); }
        }

        /// <summary>
        /// All the Output nodes in this chain
        /// </summary>
        public IEnumerable<OutputNode> Outputs
        {
            get { return this.OfType<OutputNode>(); }
        }

        public IRouteDefinition Route { get; set; }

        public AuthorizationNode Authorization { get; private set; }




        public IEnumerator<BehaviorNode> GetEnumerator()
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

        internal void SetTop(BehaviorNode node)
        {
            node.Previous = null;

            if (_top != null)
            {
                _top.Chain = null;
            }

            _top = node;
            node.Chain = this;
        }

        public void PartialOnly()
        {
            Route = new NulloRouteDefinition();
        }

        public bool IsPartialOnly()
        {
            return Route is NulloRouteDefinition;
        }

        /// <summary>
        /// Tests whether or not this chain has any output nodes
        /// </summary>
        /// <returns></returns>
        public bool HasOutputBehavior()
        {
            return Top == null ? false : Top.HasOutputBehavior();
        }

        /// <summary>
        /// Prepends the prefix to the route definition
        /// </summary>
        /// <param name="prefix"></param>
        public void PrependToUrl(string prefix)
        {
            if (Route != null)
            {
                Route.Prepend(prefix);
            }
        }

        /// <summary>
        /// Adds a new BehaviorNode to the very end of this behavior chain
        /// </summary>
        /// <param name="node"></param>
        public void AddToEnd(BehaviorNode node)
        {
            if (Top == null)
            {
                SetTop(node);
                return;
            }

            Top.AddToEnd(node);
        }

        /// <summary>
        /// Adds a new BehaviorNode of type T to the very end of this
        /// behavior chain
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddToEnd<T>() where T : BehaviorNode, new()
        {
            var node = new T();
            AddToEnd(node);
            return node;
        }

        // TODO -- this should return the *last* output model, not the first
        public Type ActionOutputType()
        {
            var call = Calls.FirstOrDefault();
            return call == null ? null : call.OutputType();
        }

        // TODO -- move this to an interface implementation
        public void Register(Action<Type, ObjectDef> callback)
        {
            callback(typeof (IActionBehavior), Top.ToObjectDef());
            Authorization.Register(Top.UniqueId, callback);
        }

        /// <summary>
        /// Sets the specified BehaviorNode as the outermost node
        /// in this chain
        /// </summary>
        /// <param name="node"></param>
        public void Prepend(BehaviorNode node)
        {
            var next = Top;
            SetTop(node);

            if (next != null)
            {
                Top.Next = next;
            }
        }

        /// <summary>
        /// The first ActionCall in this BehaviorChain.  Can be null.
        /// </summary>
        /// <returns></returns>
        public ActionCall FirstCall()
        {
            return Calls.FirstOrDefault();
        }

        /// <summary>
        /// Returns the InputType of the very first 
        /// </summary>
        /// <returns></returns>
        public Type InputType()
        {
            var inputTypeHolder = this.OfType<IMayHaveInputType>().FirstOrDefault();
            return inputTypeHolder == null ? null : inputTypeHolder.InputType();
        }


        public bool HasInput()
        {
            return InputType() != null;
        }

        /// <summary>
        /// Creates a new BehaviorChain for an action method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static BehaviorChain For<T>(Expression<Action<T>> expression)
        {
            var call = ActionCall.For(expression);
            var chain = new BehaviorChain();
            chain.AddToEnd(call);

            return chain;
        }

        /// <summary>
        /// Checks to see if a Wrapper node of the requested behaviorType anywhere in the chain
        /// regardless of position
        /// </summary>
        /// <param name="behaviorType"></param>
        /// <returns></returns>
        public bool IsWrappedBy(Type behaviorType)
        {
            return this.Where(x => x is Wrapper).Cast<Wrapper>().Any(x => x.BehaviorType == behaviorType);
        }
    }
}