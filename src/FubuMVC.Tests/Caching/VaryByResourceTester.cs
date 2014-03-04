using System.Collections.Generic;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using NUnit.Framework;
using System.Linq;
using FubuCore;
using FubuTestingSupport;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class VaryByResourceTester
    {
        [Test]
        public void hash_values_when_the_chain_has_a_route_but_not_real_values()
        {
            var chain = new RoutedChain(new RouteDefinition("some/pattern/url"));
        

            var currentChain = new CurrentChain(chain, new Dictionary<string, object>());

            var varyBy = new VaryByResource(currentChain);

            var values = varyBy.Values();
            values.Select(x => "{0}={1}".ToFormat(x.Key, x.Value)).ShouldHaveTheSameElementsAs("chain=" + chain.GetRoutePattern());
        }

        [Test]
        public void hash_values_with_a_route_That_has_substitutions()
        {
            var chain = new RoutedChain(RouteBuilder.Build<Query>("some/pattern/url/{from}/{to}"));

            var currentChain = new CurrentChain(chain, new Dictionary<string, object>{{"from", 1}, {"to", 2}});
            var varyBy = new VaryByResource(currentChain);

            var values = varyBy.Values();
            values.Select(x => "{0}={1}".ToFormat(x.Key, x.Value))
                .ShouldHaveTheSameElementsAs("chain=" + "some/pattern/url/{from}/{to}", "from=1", "to=2");
        }

        [Test]
        public void hash_values_with_a_chain_that_is_a_partial()
        {
            var chain = new BehaviorChain()
            {

            };

            var currentChain = new CurrentChain(chain, new Dictionary<string, object> { { "from", 1 }, { "to", 2 } });
            var varyBy = new VaryByResource(currentChain);

            var values = varyBy.Values();
            values.Select(x => "{0}={1}".ToFormat(x.Key, x.Value))
                .ShouldHaveTheSameElementsAs("chain=" + chain.ToString());
        }

        public class Query
        {
            public string from { get; set; }
            public string to { get; set; }
        }
    }
}