using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class TagAttributeTester
    {
        [Test]
        public void applies_the_tags()
        {
            var graph = BehaviorGraph.BuildFrom(x => x.Actions.IncludeType<TaggedEndpoint>());
            graph.BehaviorFor<TaggedEndpoint>(x => x.get_tags())
                .Tags.ShouldHaveTheSameElementsAs("foo", "bar");
        }
    }

    public class TaggedEndpoint
    {
        [Tag("foo", "bar")]
        public string get_tags()
        {
            return "some tags";
        }
    }
}