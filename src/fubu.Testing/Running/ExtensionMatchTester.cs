using System.IO;
using Fubu.Running;
using NUnit.Framework;
using FubuTestingSupport;

namespace fubu.Testing.Running
{
    [TestFixture]
    public class ExtensionMatchTester
    {
        [Test]
        public void matches()
        {
            var match = new ExtensionMatch(FileChangeCategory.AppDomain, "*.spark");

            match.Matches("foo.spark").ShouldBeTrue();
            match.Matches("foo.bar").ShouldBeFalse();
        }


    }
}