using System.Linq;
using FubuCore;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Packaging
{
    [TestFixture, Ignore("passes, but it's too unreliable for CI")]
    public class unlinking_a_bottle_from_the_command_line : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runBottles(@"
link harness --clean-all
init src/TestPackage1 pak1
create pak1 -o pak1.zip
");

            installZipPackage("pak1.zip");
        }

        [Test]
        public void load_actions_from_a_bottle_before_and_after()
        {

            var expectation =
                @"
StringController.SayHello()
JsonController.SendMessage()
ViewController.ShowView()
OneController.Report()
OneController.Query()
TwoController.Report()
TwoController.Query()
ThreeController.Report()
ThreeController.Query()
"
                    .ReadLines().Where(x => x.IsNotEmpty()).OrderBy(x => x);

            remote.All().EndpointsForAssembly("TestPackage1").Select(x => x.FirstActionDescription).OrderBy(x => x).ShouldHaveTheSameElementsAs(expectation);

            uninstallZipPackage("pak1.zip");

            restart();

            remote.All().EndpointsForAssembly("TestPackage1")
                .Any().ShouldBeFalse();
        }

    }
}