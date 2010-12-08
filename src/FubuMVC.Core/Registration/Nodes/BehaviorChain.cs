using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Registration.Nodes
{
    public class BehaviorChain : BehaviorNode
    {
        public BehaviorChain()
        {
            Authorization = new AuthorizationNode();
            UrlCategory = new UrlCategory();
        }

        public UrlCategory UrlCategory { get; private set; }

        public BehaviorNode Top
        {
            get { return Next; }
            private set { Next = value; }
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Chain; }
        }

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

        public string FirstCallDescription
        {
            get
            {
                var call = FirstCall();
                return call == null ? string.Empty : call.Description;
            }
        }

        public string RoutePattern
        {
            get { return Route == null ? string.Empty : Route.Pattern; }
        }




        public void PrependToUrl(string prefix)
        {
            if (Route != null)
            {
                Route.Prepend(prefix);
            }
        }

        public override void AddToEnd(BehaviorNode node)
        {
            if (Top == null)
            {
                Top = node;
                return;
            }

            var last = this.OfType<BehaviorNode>().LastOrDefault();
            if (last != null)
            {
                last.AddAfter(node);
            }
        }

        public T AddToEnd<T>() where T : BehaviorNode, new()
        {
            var node = new T();
            AddToEnd(node);
            return node;
        }

        public Type ActionOutputType()
        {
            var call = Calls.FirstOrDefault();
            return call == null ? null : call.OutputType();
        }

        public override ObjectDef ToObjectDef()
        {
            // TODO -- throw if there is no Top.  Invalid state

            var def = Top.ToObjectDef();
            def.Name = UniqueId.ToString();
            return def;
        }

        public void Register(Action<Type, ObjectDef> callback)
        {
            callback(typeof (IActionBehavior), ToObjectDef());
            Authorization.Register(UniqueId, callback);
        }

        public void Prepend(BehaviorNode node)
        {
            if (Top == null)
            {
                Top = node;
            }
            else
            {
                Top.AddBefore(node);
                Top = node;
            }
        }

        protected override ObjectDef buildObjectDef()
        {
            // does not get called.  It's goofy.  We'll fix this at some point
            throw new NotImplementedException();
        }

        public override IEnumerator<BehaviorNode> GetEnumerator()
        {
            if (Top != null)
            {
                yield return Top;
                foreach (BehaviorNode node in Top)
                {
                    yield return node;
                }
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
    }
}