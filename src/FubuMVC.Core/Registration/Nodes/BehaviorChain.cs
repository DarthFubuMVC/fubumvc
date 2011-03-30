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
    public class BehaviorChain : IEnumerable<BehaviorNode>
    {
        public BehaviorChain()
        {
            Authorization = new AuthorizationNode();
            UrlCategory = new UrlCategory();
        }

        public Guid UniqueId
        {
            get
            {
                return Top == null ? Guid.Empty : Top.UniqueId;
            }
        }

        public UrlCategory UrlCategory { get; private set; }

        private BehaviorNode _top;
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
        
        public BehaviorNode Top
        {
            get { return _top; }
        }

        public string Origin { get; set; }

        public IEnumerable<ActionCall> Calls
        {
            get { return this.OfType<ActionCall>(); }
        }

        public IEnumerable<OutputNode> Outputs
        {
            get { return this.OfType<OutputNode>(); }
        }

        public IRouteDefinition Route { get; set; }

        public void PartialOnly()
        {
            Route = new NulloRouteDefinition();
        }

        public bool IsPartialOnly()
        {
            return Route is NulloRouteDefinition;
        }

        public AuthorizationNode Authorization { get; private set; }

        [Obsolete]
        public string FirstCallDescription
        {
            get
            {
                var call = FirstCall();
                return call == null ? string.Empty : call.Description;
            }
        }

        [Obsolete("Maybe")]
        public string RoutePattern
        {
            get { return Route == null ? string.Empty : Route.Pattern; }
        }


        public bool HasOutputBehavior()
        {
            return Top == null ? false : Top.HasOutputBehavior();
        }

        public void PrependToUrl(string prefix)
        {
            if (Route != null)
            {
                Route.Prepend(prefix);
            }
        }

        public void AddToEnd(BehaviorNode node)
        {
            if (Top == null)
            {
                SetTop(node);
                return;
            }

            Top.AddToEnd(node);
        }

        public T AddToEnd<T>() where T : BehaviorNode, new()
        {
            var node = new T();
            AddToEnd(node);
            return node;
        }

        [Obsolete("Maybe")]
        public Type ActionOutputType()
        {
            var call = Calls.FirstOrDefault();
            return call == null ? null : call.OutputType();
        }

 
        public void Register(Action<Type, ObjectDef> callback)
        {
            callback(typeof (IActionBehavior), Top.ToObjectDef());
            Authorization.Register(Top.UniqueId, callback);
        }

        public void Prepend(BehaviorNode node)
        {
            var next = Top;
            SetTop(node);

            if (next != null)
            {
                Top.Next = next;
            }
        }


        public ActionCall FirstCall()
        {
            return Calls.FirstOrDefault();
        }

        public bool ContainsCall(Func<ActionCall, bool> filter)
        {
            return Calls.Any(filter);
        }

        public Type InputType()
        {
            if (Top == null) return null;

            var inputTypeHolder =  this.OfType<IMayHaveInputType>().FirstOrDefault();
            return inputTypeHolder == null ? null : inputTypeHolder.InputType();
        }


        public bool HasInput()
        {
            return InputType() != null;
        }

        public string InputTypeName
        {
            get
            {
                var type = InputType();
                return type == null ? string.Empty : type.Name;
            }
        }

        public static BehaviorChain For<T>(Expression<Action<T>> expression)
        {
            var call = ActionCall.For(expression);
            var chain = new BehaviorChain();
            chain.AddToEnd(call);

            return chain;
        }

        public bool IsWrappedBy(Type behaviorType)
        {
            return this.Where(x => x is Wrapper).Cast<Wrapper>().Any(x => x.BehaviorType == behaviorType);
        }

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
    }
}