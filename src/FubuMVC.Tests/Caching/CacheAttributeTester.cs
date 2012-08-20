using System;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using HtmlTags;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class CacheAttributeTester
    {
        [Test]
        public void varies_by_resource_only_by_default()
        {
            var att = new CacheAttribute();

            att.VaryBy.ShouldBeNull();
        }

        [Test]
        public void alter_chain()
        {
            var att = new CacheAttribute();
            var chain = new BehaviorChain();
            var call = ActionCall.For<CacheAttributeTester>(x => x.alter_chain());
            chain.AddToEnd(call);

            att.Alter(call);

            chain.OfType<OutputCachingNode>().Single()
                .VaryByPolicies().Single().ShouldEqual(typeof (VaryByResource));
        }

        [Test]
        public void alter_chain_is_idempotent()
        {
            var att = new CacheAttribute();
            var chain = new BehaviorChain();
            var call = ActionCall.For<CacheAttributeTester>(x => x.alter_chain());
            chain.AddToEnd(call);

            att.Alter(call);
            att.Alter(call);
            att.Alter(call);
            att.Alter(call);
            att.Alter(call);

            chain.OfType<OutputCachingNode>().Single()
                .VaryByPolicies().Single().ShouldEqual(typeof(VaryByResource));
        }

        [Test]
        public void alter_chain_with_more_overridden_vary_by()
        {
            var att = new CacheAttribute();
            att.VaryBy = new Type[]{typeof(VaryByResource), typeof(VaryByThreadCulture)};

            var chain = new BehaviorChain();
            var call = ActionCall.For<CacheAttributeTester>(x => x.alter_chain());
            chain.AddToEnd(call);

            att.Alter(call);

            chain.OfType<OutputCachingNode>().Single().VaryByPolicies()
                .ShouldHaveTheSameElementsAs(typeof(VaryByResource), typeof(VaryByThreadCulture));
        }

        [Test]
        public void integrated_test_of_configuring_output_cache()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<FakeEndpoint>();
            });

            var chain = graph.BehaviorFor<FakeEndpoint>(x => x.get_this_one());

            chain.OfType<OutputNode>().Single().Previous.ShouldBeOfType<OutputCachingNode>();
        }


        public class FakeEndpoint
        {
            [Cache]
            public HtmlTag get_this_one()
            {
                return new HtmlTag("Look here!");
            }
        }
    }
}