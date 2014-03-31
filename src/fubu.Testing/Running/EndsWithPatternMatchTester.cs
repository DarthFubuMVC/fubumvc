using Fubu.Running;
using NUnit.Framework;
using FubuTestingSupport;

namespace fubu.Testing.Running
{
    [TestFixture]
    public class EndsWithPatternMatchTester
    {
        [Test]
        public void matches()
        {
            var match = new EndsWithPatternMatch(FileChangeCategory.AppDomain, "*.asset.config");

            match.Matches("something.asset.config").ShouldBeTrue();
            match.Matches("something.Asset.config").ShouldBeTrue();
            match.Matches("foo.asset.config").ShouldBeTrue();
            match.Matches("bar.asset.config").ShouldBeTrue();
            match.Matches("something.foo.config").ShouldBeFalse();
            match.Matches("something.asset.bar").ShouldBeFalse();
        }
    }
}