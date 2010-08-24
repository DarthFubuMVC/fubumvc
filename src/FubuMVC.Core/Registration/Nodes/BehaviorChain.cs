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
    public class UrlCategory
    {
        public UrlCategory()
        {
            Creates = new List<Type>();
        }

        public string Category { get; set; }
        public IList<Type> Creates { get; private set; }
    }

    public class BehaviorChain : BehaviorNode
    {
        public BehaviorChain()
        {
            Authorization = new AuthorizationNode();
            UrlCategory = new UrlCategory();
        }

        public UrlCategory UrlCategory { get; private set; }

        public BehaviorNode Top { get { return Next; } private set { Next = value; } }

        public override BehaviorCategory Category { get { return BehaviorCategory.Chain; } }

        public IEnumerable<ActionCall> Calls { get { return this.OfType<ActionCall>(); } }
        public IEnumerable<OutputNode> Outputs { get { return this.OfType<OutputNode>(); } }

        public IRouteDefinition Route { get; set; }

        public AuthorizationNode Authorization { get; private set; }

        public string FirstCallDescription
        {
            get
            {
                ActionCall call = FirstCall();
                return call == null ? string.Empty : call.Description;
            }
        }

        public string RoutePattern { get { return Route == null ? string.Empty : Route.Pattern; } }

        public string InputTypeName
        {
            get
            {
                ActionCall call = FirstCall();
                return call == null || call.InputType() == null ? string.Empty : call.InputType().Name;
            }
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

            BehaviorNode last = this.OfType<BehaviorNode>().LastOrDefault();
            if (last != null)
            {
                last.AddAfter(node);
            }
        }

        public Type ActionOutputType()
        {
            ActionCall call = Calls.FirstOrDefault();
            return call == null ? null : call.OutputType();
        }

        public override ObjectDef ToObjectDef()
        {
            // TODO -- throw if there is no Top.  Invalid state

            ObjectDef def = Top.ToObjectDef();
            def.Name = UniqueId.ToString();
            return def;
        }

        public void Register(Action<Type, ObjectDef> callback)
        {
            callback(typeof (IActionBehavior), ToObjectDef());
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

        public Type ActionInputType()
        {
            ActionCall call = FirstCall();
            return call == null ? null : call.InputType();
        }


        public bool HasInput()
        {
            ActionCall call = FirstCall();
            return call == null ? false : call.HasInput;
        }

        public static BehaviorChain For<T>(Expression<Action<T>> expression)
        {
            ActionCall call = ActionCall.For(expression);
            var chain = new BehaviorChain();
            chain.AddToEnd(call);

            return chain;
        }
    }
}