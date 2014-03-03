using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Policies;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class ResourceTypeIsTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            theGraph = BehaviorGraph.BuildFrom(x => {
                x.Actions.IncludeType<ResourceTypeEndpoints>();
            });
        }

        [Test]
        public void matches_positive()
        {
            new ResourceTypeIs<string>().Matches(theGraph.BehaviorFor<ResourceTypeEndpoints>(x => x.get_hello()))
                .ShouldBeTrue();
        }

        [Test]
        public void matches_negative()
        {
            new ResourceTypeIs<string>().Matches(theGraph.BehaviorFor<ResourceTypeEndpoints>(x => x.get_goodbye()))
                .ShouldBeFalse();
        }

        [Test]
        public void does_not_blow_up_if_The_resource_type_is_null()
        {
            new ResourceTypeIs<string>().Matches(theGraph.BehaviorFor<ResourceTypeEndpoints>(x => x.post_name(null)))
                .ShouldBeFalse();
        }
    }

    public class ResourceTypeEndpoints
    {
        public void post_name(ResourceTypeModel input)
        {
            
        }

        public string get_hello()
        {
            return "Hello";
        }

        public ResourceTypeModel get_goodbye()
        {
            return null;
        }
    }

    public class ResourceTypeModel{}
}