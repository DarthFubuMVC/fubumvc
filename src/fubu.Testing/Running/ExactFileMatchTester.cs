using Fubu.Running;
using NUnit.Framework;
using FubuTestingSupport;

namespace fubu.Testing.Running
{
    [TestFixture]
    public class ExactFileMatchTester
    {
        [Test]
        public void matches()
        {
            var match = new ExactFileMatch(FileChangeCategory.AppDomain, "web.config");

            match.Matches("web.config").ShouldBeTrue();
            match.Matches("Web.config").ShouldBeTrue();
            match.Matches("foo.config").ShouldBeFalse();
            match.Matches("bar.config").ShouldBeFalse();
        }
    }
}