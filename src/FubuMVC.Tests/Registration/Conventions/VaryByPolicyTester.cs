using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class VaryByPolicyTester
    {
        [Test]
        public void vary_by_thread_culture()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<CachedController>();
                x.Policies.Add(new VaryByPolicy().Apply<VaryByThreadCulture>());
            });

            graph.BehaviorFor<CachedController>(x => x.OneCachedPartial())
                .OfType<OutputCachingNode>().Single()
                .VaryByPolicies().ShouldContain(typeof(VaryByThreadCulture));
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