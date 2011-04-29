using System.IO;
using System.Linq;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests
{
    [TestFixture]
    public class SolutionDirectoryPackageLoaderTester
    {

        [Test]
        public void there_are_7_manifests_that_are_modules_in_fubu()
        {
            // setup in a random directory
            var p = Path.GetFullPath(@"..\..\..");
            var sdpl = new SolutionDirectoryPackageLoader(p);
            var pis = sdpl.Load();

            
            pis.Count().ShouldEqual(7);
        }
    }
}