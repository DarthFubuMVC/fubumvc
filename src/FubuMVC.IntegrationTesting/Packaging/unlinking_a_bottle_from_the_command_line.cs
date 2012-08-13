using System.Linq;
using FubuCore;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Packaging
{
    [TestFixture]
    public class unlinking_a_bottle_from_the_command_line : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runFubu("packages harness --clean-all");

            runBottles(@"
link harness --clean-all
init src/TestPackage1 pak1
create pak1 -o pak1.zip
");

            runFubu("install-pak pak1.zip harness");
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


            runFubu("install-pak pak1.zip harness -u");
            restart();

            remote.All().EndpointsForAssembly("TestPackage1")
                .Any().ShouldBeFalse();
        }

    }
}