using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class CachedPartialConventionTester
    {
        [Test]
        public void modify_without_any_prior_caching()
        {
            var chain = new BehaviorChain();

            CachedPartialConvention.Modify(chain);

            chain.Last().ShouldBeOfType<OutputCachingNode>()
                .VaryByPolicies().Single().ShouldEqual(typeof (VaryByResource));
        }

        [Test]
        public void modify_is_idempotent()
        {
            var chain = new BehaviorChain();

            CachedPartialConvention.Modify(chain);
            CachedPartialConvention.Modify(chain);
            CachedPartialConvention.Modify(chain);
            CachedPartialConvention.Modify(chain);

            chain.Last().ShouldBeOfType<OutputCachingNode>()
                .VaryByPolicies().Single().ShouldEqual(typeof(VaryByResource));
        }

        [Test]
        public void ShouldBeCachedPartial()
        {
            CachedPartialConvention.ShouldBeCachedPartial(ActionCall.For<CachedController>(x => x.OneCachedPartial())).ShouldBeTrue();
            CachedPartialConvention.ShouldBeCachedPartial(ActionCall.For<CachedController>(x => x.TwoPartial())).ShouldBeFalse();
            CachedPartialConvention.ShouldBeCachedPartial(ActionCall.For<CachedController>(x => x.Cached())).ShouldBeFalse();
            CachedPartialConvention.ShouldBeCachedPartial(ActionCall.For<CachedController>(x => x.Something())).ShouldBeFalse();
        }

        [Test]
        public void integrated_test()
        {
            var graph = BehaviorGraph.BuildFrom(x => x.Actions.IncludeType<CachedController>());

            graph.BehaviorFor<CachedController>(x => x.TwoPartial()).OfType<OutputCachingNode>().Any().ShouldBeFalse();
            graph.BehaviorFor<CachedController>(x => x.Cached()).OfType<OutputCachingNode>().Any().ShouldBeFalse();
            graph.BehaviorFor<CachedController>(x => x.Something()).OfType<OutputCachingNode>().Any().ShouldBeFalse();

            var chain1 = graph.BehaviorFor<CachedController>(x => x.OneCachedPartial());
            var cacheNode = chain1.OfType<OutputCachingNode>().Single();

            cacheNode.ShouldNotBeNull();
            cacheNode.Next.ShouldBeOfType<OutputNode>();
        }

        public class CachedController
        {
            public string OneCachedPartial()
            {
                return "hello";
            }

            public string TwoPartial()
            {
                return "Bye";
            }

            public string Something()
            {
                return "nothing";
            }

            public string Cached()
            {
                return "not this";
            }
        }
    }


}