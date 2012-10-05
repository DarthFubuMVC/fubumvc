using FubuMVC.Core.Assets;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class FileDependencyTester
    {
        [Test]
        public void should_be_after_is_false_for_unrelated_scripts()
        {
            var s1 = new FileDependency("1");
            var s2 = new FileDependency("2");
        
            s1.MustBeAfter(s2).ShouldBeFalse();
            s2.MustBeAfter(s1).ShouldBeFalse();
        }


        [Test]
        public void should_be_after_a_direct_dependency()
        {
            var s1 = new FileDependency("1");
            var s2 = new FileDependency("2");

            s2.AddDependency(s1);

            s1.MustBeAfter(s2).ShouldBeFalse();
            s2.MustBeAfter(s1).ShouldBeTrue();
        }

        [Test]
        public void should_be_after_a_direct_dependency_2_deep()
        {
            var s1 = new FileDependency("1");
            var s2 = new FileDependency("2");
            var s3 = new FileDependency("3");
        
            s3.AddDependency(s2);
            s2.AddDependency(s1);

            s3.MustBeAfter(s1).ShouldBeTrue();
            s3.MustBeAfter(s2).ShouldBeTrue();
        }

        [Test]
        public void should_be_after_bouncing_through_a_set()
        {
            var s1 = new FileDependency("1");
            var s2 = new FileDependency("2");
            var s3 = new FileDependency("3");

            var set = new AssetSet();
            set.Add(s1);

            s2.AddDependency(set);
            s3.AddDependency(s2);

            s3.MustBeAfter(s1).ShouldBeTrue();
            s3.MustBeAfter(s2).ShouldBeTrue();
        }

        [Test]
        public void extender_scenario()
        {
            var s1 = new FileDependency("1");
            var s2 = new FileDependency("2");

            s2.AddDependency(s1);

            s2.MustBeAfter(s1).ShouldBeTrue();
            s1.MustBeAfter(s2).ShouldBeFalse();
        }
    }
}