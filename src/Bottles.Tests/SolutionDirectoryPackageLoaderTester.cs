using System.Linq;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests
{
    [TestFixture]
    public class SolutionDirectoryPackageLoaderTester
    {

        [Test]
        public void NAME()
        {
            // setup in a random directory

            var sdpl = new SolutionDirectoryPackageLoader(@"C:\dev\fubumvc\src\");
            var pis = sdpl.Load();

            
            pis.Count().ShouldEqual(7);
        }
    }
}