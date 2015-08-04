using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class FubuTransportRegistry_application_of_policies_Tester
    {
        [Test]
        public void policies_can_be_applied_locally()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Handlers.DisableDefaultHandlerSource();

                x.Import<BlueRegistry>();
                x.Import<GreenRegistry>();
                x.Policies.ChainSource<SystemLevelHandlers>(); // This would be here by extensions normally
            });

            var cache = new ChainResolutionCache(graph);

            cache.FindUniqueByType(typeof (Message1)).IsWrappedBy(typeof (GreenWrapper)).ShouldBeTrue();
            cache.FindUniqueByType(typeof (Message2)).IsWrappedBy(typeof (GreenWrapper)).ShouldBeTrue();
            cache.FindUniqueByType(typeof (Message3)).IsWrappedBy(typeof (GreenWrapper)).ShouldBeTrue();

            cache.FindUniqueByType(typeof (Message4)).IsWrappedBy(typeof (GreenWrapper)).ShouldBeFalse();
            cache.FindUniqueByType(typeof (Message5)).IsWrappedBy(typeof (GreenWrapper)).ShouldBeFalse();
            cache.FindUniqueByType(typeof (Message6)).IsWrappedBy(typeof (GreenWrapper)).ShouldBeFalse();

            cache.FindUniqueByType(typeof (Message1)).IsWrappedBy(typeof (BlueWrapper)).ShouldBeFalse();
            cache.FindUniqueByType(typeof (Message2)).IsWrappedBy(typeof (BlueWrapper)).ShouldBeFalse();
            cache.FindUniqueByType(typeof (Message3)).IsWrappedBy(typeof (BlueWrapper)).ShouldBeFalse();

            cache.FindUniqueByType(typeof (Message4)).IsWrappedBy(typeof (BlueWrapper)).ShouldBeTrue();
            cache.FindUniqueByType(typeof (Message5)).IsWrappedBy(typeof (BlueWrapper)).ShouldBeTrue();
            cache.FindUniqueByType(typeof (Message6)).IsWrappedBy(typeof (BlueWrapper)).ShouldBeTrue();
        }

        [Test]
        public void global_policies_on_all_handlers()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Handlers.DisableDefaultHandlerSource();

                x.Import<BlueRegistry>();
                x.Import<GreenRegistry>();
                x.Import<RedRegistry>();

                x.Policies.ChainSource<SystemLevelHandlers>(); 
            });

            graph.Chains.Any(x => x is HandlerChain).ShouldBeTrue();
            graph.Chains.OfType<HandlerChain>()
                .Each(chain => { chain.IsWrappedBy(typeof (RedWrapper)).ShouldBeTrue(); });
        }

        [Test]
        public void does_not_apply_policies_to_fubumvc_handlers()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Handlers.DisableDefaultHandlerSource();

                x.Import<BlueRegistry>();
                x.Import<GreenRegistry>();
                x.Import<RedRegistry>();

                x.Policies.ChainSource<SystemLevelHandlers>(); 
            });

            graph.ChainFor<SomethingEndpoint>(x => x.get_hello())
                .IsWrappedBy(typeof (RedWrapper))
                .ShouldBeFalse();
        }
    }

    public class RedRegistry : FubuPackageRegistry
    {
        public RedRegistry()
        {
            Actions.DisableDefaultActionSource();

            Actions.FindBy(x => x.ExcludeTypes(_ => true));
            Policies.Global.Add<WrapPolicy<RedWrapper>>();
        }
    }

    public class GreenRegistry : FubuPackageRegistry
    {
        public GreenRegistry()
        {
            Actions.DisableDefaultActionSource();

            Actions.FindBy(x => x.ExcludeTypes(_ => true));
            Handlers.DisableDefaultHandlerSource();
            Handlers.Include<GreenHandler>();
            Policies.Local.Add<WrapPolicy<GreenWrapper>>();
        }
    }

    public class BlueRegistry : FubuPackageRegistry
    {
        public BlueRegistry()
        {
            Actions.FindBy(x => x.ExcludeTypes(_ => true));
            Actions.DisableDefaultActionSource();

            Handlers.DisableDefaultHandlerSource();
            Handlers.Include<BlueHandler>();
            Policies.Local.Add<WrapPolicy<BlueWrapper>>();
        }
    }

    public class GreenHandler
    {
        public void Handle(Message1 message)
        {
        }

        public void Handle(Message2 message)
        {
        }

        public void Handle(Message3 message)
        {
        }
    }

    public class BlueHandler
    {
        public void Handle(Message4 message)
        {
        }

        public void Handle(Message5 message)
        {
        }

        public void Handle(Message6 message)
        {
        }
    }

    public class WrapPolicy<T> : HandlerChainPolicy where T : IActionBehavior
    {
        public override void Configure(HandlerChain handlerChain)
        {
            handlerChain.InsertFirst(new Wrapper(typeof (T)));
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