using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using HtmlTags;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class default_action_discovery
    {
        [Test]
        public void with_no_other_explicit_action_discovery()
        {
            var graph = BehaviorGraph.BuildEmptyGraph();


            graph.ChainFor<MyEndpoint>(x => x.Go()).ShouldNotBeNull();
            graph.ChainFor<MyEndpoints>(x => x.Go()).ShouldNotBeNull();
            graph.ChainFor<MyEndpoints>(x => x.Go2()).ShouldNotBeNull();
        }

        [Test]
        public void does_not_find_endpoints_with_an_explicit_action_discovery()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.DisableDefaultActionSource();
                x.Actions.IncludeClassesSuffixedWithController();
            });

            graph.ChainFor<MyEndpoint>(x => x.Go()).ShouldBeNull();
            graph.ChainFor<MyEndpoints>(x => x.Go()).ShouldBeNull();
            graph.ChainFor<MyEndpoints>(x => x.Go2()).ShouldBeNull();
        }

        [Test]
        public void does_not_find_endpoints_with_an_explicit_action_discovery_policy()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.DisableDefaultActionSource();
                x.Actions.FindWith(new FakeActionSource());
            });

            graph.ChainFor<MyEndpoint>(x => x.Go()).ShouldBeNull();
            graph.ChainFor<MyEndpoints>(x => x.Go()).ShouldBeNull();
            graph.ChainFor<MyEndpoints>(x => x.Go2()).ShouldBeNull();
        }

        [Test]
        public void will_find_endpoints_if_endpoint_is_explictly_specified_too()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.FindWith(new FakeActionSource());
                x.Actions.IncludeClassesSuffixedWithEndpoint();
            });

            graph.ChainFor<MyEndpoint>(x => x.Go()).ShouldNotBeNull();
            graph.ChainFor<MyEndpoints>(x => x.Go()).ShouldNotBeNull();
            graph.ChainFor<MyEndpoints>(x => x.Go2()).ShouldNotBeNull();
        }
    }

    public class FakeActionSource : IActionSource
    {
        public Task<ActionCall[]> FindActions(Assembly applicationAssembly)
        {
            return Task.FromResult(new ActionCall[0]);
        }
    }

    public class MyEndpoint
    {
        public HtmlTag Go()
        {
            return new HtmlTag("div");
        }
    }

    public class MyEndpoints
    {
        public HtmlTag Go()
        {
            return new HtmlTag("div");
        }

        public HtmlTag Go2()
        {
            return new HtmlTag("div");
        }
    }
}