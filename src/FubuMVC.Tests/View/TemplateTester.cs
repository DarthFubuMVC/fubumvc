using NUnit.Framework;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class TemplateTester
    {
        [Test]
        public void Do()
        {
            Assert.Fail("Add more tests for this little monster");
        }

        [Test]
        public void can_get_shared_view_paths_for_origin()
        {
            Assert.Fail("UT for origin ViewPath");
//            var policy = new ViewPathPolicy<ISparkTemplate>();
//            _templates.Each(policy.Apply);
//            var origin = templateAt(1).Origin; //from pak1 which has dependency on pak2
//            var pak2SharedLocation = FileSystem.Combine("_Pak2", "Home", Shared);
//            ClassUnderTest.SharedViewPathsForOrigin(origin).ShouldContain(pak2SharedLocation);
        }

        [Test]
        public void create_view_path_for_bottle_sourced_view()
        {
            Assert.Fail("Do.");
        }

        [Test]
        public void determine_namespace_at_the_root_of_the_assembly()
        {
            Assert.Fail("Do.");
        }

        [Test]
        public void determine_namespace_when_you_are_not_at_the_root_of_the_assembly()
        {
            Assert.Fail("Do.");
        }
    }
}