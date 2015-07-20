using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuTransportation.Configuration;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using System.Collections.Generic;
using Message1 = FubuTransportation.Testing.Message1;
using Message2 = FubuTransportation.Testing.Message2;
using Message3 = FubuTransportation.Testing.Message3;
using Message4 = FubuTransportation.Testing.Message4;
using Message5 = FubuTransportation.Testing.Message5;
using Message6 = FubuTransportation.Testing.Message6;

namespace FubuTransportation.Testing
{
    [TestFixture]
    public class FubuTransportRegistry_application_of_policies_Tester
    {
        

        [Test]
        public void policies_can_be_applied_locally()
        {
            var graph = BehaviorGraph.BuildFrom(x => {
                x.Import<BlueRegistry>();
                x.Import<GreenRegistry>();
                x.Policies.ChainSource<ImportHandlers>(); // This would be here by Bottles normally
            });

            var cache = new ChainResolutionCache(graph);

            cache.FindUniqueByType(typeof(Message1)).IsWrappedBy(typeof(GreenWrapper)).ShouldBeTrue();
            cache.FindUniqueByType(typeof(Message2)).IsWrappedBy(typeof(GreenWrapper)).ShouldBeTrue();
            cache.FindUniqueByType(typeof(Message3)).IsWrappedBy(typeof(GreenWrapper)).ShouldBeTrue();

            cache.FindUniqueByType(typeof(Message4)).IsWrappedBy(typeof(GreenWrapper)).ShouldBeFalse();
            cache.FindUniqueByType(typeof(Message5)).IsWrappedBy(typeof(GreenWrapper)).ShouldBeFalse();
            cache.FindUniqueByType(typeof(Message6)).IsWrappedBy(typeof(GreenWrapper)).ShouldBeFalse();

            cache.FindUniqueByType(typeof(Message1)).IsWrappedBy(typeof(BlueWrapper)).ShouldBeFalse();
            cache.FindUniqueByType(typeof(Message2)).IsWrappedBy(typeof(BlueWrapper)).ShouldBeFalse();
            cache.FindUniqueByType(typeof(Message3)).IsWrappedBy(typeof(BlueWrapper)).ShouldBeFalse();

            cache.FindUniqueByType(typeof(Message4)).IsWrappedBy(typeof(BlueWrapper)).ShouldBeTrue();
            cache.FindUniqueByType(typeof(Message5)).IsWrappedBy(typeof(BlueWrapper)).ShouldBeTrue();
            cache.FindUniqueByType(typeof(Message6)).IsWrappedBy(typeof(BlueWrapper)).ShouldBeTrue();
        }

        [Test]
        public void global_policies_on_all_handlers()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Import<BlueRegistry>();
                x.Import<GreenRegistry>();
                x.Import<RedRegistry>();

                x.Policies.ChainSource<ImportHandlers>(); // This would be here by Bottles normally
            });

            graph.Behaviors.Any(x => x is HandlerChain).ShouldBeTrue();
            graph.Behaviors.OfType<HandlerChain>().Each(chain => {
                chain.IsWrappedBy(typeof (RedWrapper)).ShouldBeTrue();
            });
        }

        [Test]
        public void does_not_apply_policies_to_fubumvc_handlers()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Import<BlueRegistry>();
                x.Import<GreenRegistry>();
                x.Import<RedRegistry>();

                x.Policies.ChainSource<ImportHandlers>(); // This would be here by Bottles normally
            });

            graph.BehaviorFor<SomethingEndpoint>(x => x.get_hello())
                .IsWrappedBy(typeof(RedWrapper))
                .ShouldBeFalse();
        }
    }

    public class RedRegistry : FubuTransportRegistry
    {
        public RedRegistry()
        {
            Global.Policy<WrapPolicy<RedWrapper>>();
        }
    }

    public class GreenRegistry : FubuTransportRegistry
    {
        public GreenRegistry()
        {
            Handlers.DisableDefaultHandlerSource();
            Handlers.Include<GreenHandler>();
            Local.Policy<WrapPolicy<GreenWrapper>>();
        }
    }

    public class BlueRegistry : FubuTransportRegistry
    {
        public BlueRegistry()
        {
            Handlers.DisableDefaultHandlerSource();
            Handlers.Include<BlueHandler>();
            Local.Policy<WrapPolicy<BlueWrapper>>();
        }
    }

    public class GreenHandler
    {
        public void Handle(Message1 message){}
        public void Handle(Message2 message){}
        public void Handle(Message3 message){}
    }

    public class BlueHandler
    {
        public void Handle(Message4 message) { }
        public void Handle(Message5 message) { }
        public void Handle(Message6 message) { }
    }

    public class WrapPolicy<T> : HandlerChainPolicy where T : IActionBehavior
    {
        public override void Configure(HandlerChain handlerChain)
        {
            handlerChain.InsertFirst(new Wrapper(typeof(T)));
        }
    }

    public class GreenWrapper : WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            action();
        }
    }

    public class BlueWrapper : WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            action();
        }
    }

    public class RedWrapper : WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            action();
        }
    }

    public class FakeBehavior4 : WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            action();
        }
    }

    public class SomethingEndpoint
    {
        public string get_hello()
        {
            return "hello";
        }
    }
}