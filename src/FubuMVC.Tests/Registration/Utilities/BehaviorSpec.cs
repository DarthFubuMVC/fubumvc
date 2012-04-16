using System;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Tests.Registration.Utilities
{
    public interface IBehaviorSpec
    {
        void Verify(BehaviorSpecCheck check, BehaviorNode node);
    }

    public abstract class BehaviorSpec<T> : IBehaviorSpec where T : BehaviorNode
    {
        public void Verify(BehaviorSpecCheck check, BehaviorNode node)
        {
            check.Write(ToString());

            if (node == null)
            {
                check.RegisterError("was null");
                return;
            }

            var specific = node as T;
            if (specific == null)
            {
                check.RegisterError("was " + node.GetType().Name);
                return;
            }

            specificChecks(specific, check);
        }

        public void Verify(BehaviorChain chain)
        {
            var check = new BehaviorSpecCheck();
            Verify(check, chain.Top);


            check.AssertBehaviors();
        }

        protected abstract void specificChecks(T call, BehaviorSpecCheck check);

        protected void propagate(BehaviorSpecCheck check, IBehaviorSpec spec, BehaviorNode node)
        {
            if (spec == null && node != null)
            {
                check.RegisterError("unexpected node:  " + node);
            }

            if (spec != null)
            {
                spec.Verify(check, node);
            }
        }
    }

    public abstract class ChainedBehaviorSpec<T> : BehaviorSpec<T> where T : BehaviorNode
    {
        public IBehaviorSpec Inner { get; set; }

        protected override void specificChecks(T node, BehaviorSpecCheck check)
        {
            doSpecificCheck(node, check);
            propagate(check, Inner, node.Next);
        }

        protected abstract void doSpecificCheck(T node, BehaviorSpecCheck check);

        public ActionCallSpec Call<C>(Expression<Func<C, object>> expression)
        {
            ActionCallSpec call = ActionCallSpec.For(expression);
            Inner = call;
            return call;
        }

        public ActionCallSpec Call<C>(Expression<Action<C>> expression)
        {
            ActionCallSpec call = ActionCallSpec.For(expression);
            Inner = call;
            return call;
        }


    }
}