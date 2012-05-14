using System.Linq;
using FubuMVC.IntegrationTesting.Conneg;
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
            remote.All().EndpointsForAssembly("TestPackage1").Select(x => x.FirstActionDescription).ShouldHaveTheSameElementsAs("StringController.SayHello()", "JsonController.SendMessage()", "ViewController.ShowView()");


            runFubu("install-pak pak1.zip harness -u");
            restart();

            remote.All().EndpointsForAssembly("TestPackage1")
                .Any().ShouldBeFalse();
        }

    }
}