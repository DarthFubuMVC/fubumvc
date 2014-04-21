using System.Linq;
using FubuCore;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class CacheAttributeTester
    {
        [Test]
        public void varies_by_resource_only_by_default()
        {
            var att = new CacheAttribute();

            att.VaryBy.Any().ShouldBeFalse();
        }

        [Test, Cache]
        public void alter_chain()
        {
            var chain = new BehaviorChain();
            var call = ActionCall.For<CacheAttributeTester>(x => x.alter_chain());
            chain.AddToEnd(call);

            call.As<IModifiesChain>().Modify(chain);

            chain.OfType<OutputCachingNode>().Single()
                .VaryByPolicies().Single().ShouldEqual(typeof (VaryByResource));
        }

        [Test, Cache(typeof (VaryByResource), typeof (VaryByThreadCulture))]
        public void alter_chain_with_more_overridden_vary_by()
        {
            var chain = new BehaviorChain();
            var call = ActionCall.For<CacheAttributeTester>(x => x.alter_chain_with_more_overridden_vary_by());
            chain.AddToEnd(call);

            call.As<IModifiesChain>().Modify(chain);

            chain.OfType<OutputCachingNode>().Single().VaryByPolicies()
                .ShouldHaveTheSameElementsAs(typeof (VaryByResource), typeof (VaryByThreadCulture));
        }

        [Test]
        public void integrated_test_of_configuring_output_cache()
        {
            var graph = BehaviorGraph.BuildFrom(x => { x.Actions.IncludeType<FakeEndpoint>(); });

            var chain = graph.BehaviorFor<FakeEndpoint>(x => x.get_this_one());

            chain.OfType<OutputCachingNode>().Single().OfType<OutputNode>().Any().ShouldBeTrue();
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