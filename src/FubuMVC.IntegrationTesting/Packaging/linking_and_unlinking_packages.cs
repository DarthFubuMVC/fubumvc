using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.IntegrationTesting.Packaging
{
    [TestFixture]
    public class loading_content_and_actions_from_a_linked_package : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runBottles(@"
link harness --clean-all
            ");

            runFubu("packages harness --clean-all --remove-all");

            runBottles(@"
init src/TestPackage1 pak1
link harness pak1
");
        }

        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ScriptsHandler>();
        }

        [Test]
        public void load_actions_from_a_bottle()
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


            remote.All().EndpointsForAssembly("TestPackage1").Select(x => x.FirstActionDescription).OrderBy(x => x)
                .ShouldHaveTheSameElementsAs(expectation);
        }

        [Test]
        public void reads_asset_config_from_the_bottle()
        {
            var request = new ScriptRequest
            {
                Mandatories = "Pak1Set"
            };

            endpoints.GetByInput(request).ScriptNames()
                .ShouldHaveTheSameElementsAs(
                "_content/scripts/Pak1-A.js",
                "_content/scripts/Script1.js",
                "_content/scripts/Script2.js"
                );
        }
    }


    [TestFixture]
    public class linking_and_unlinking_packages : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runBottles(@"
link harness --clean-all
            ");

            runFubu("packages harness --clean-all --remove-all");
        }



        [Test]
        public void script_linking_and_unlinking()
        {
            remote.All().EndpointsForAssembly("TestPackage1").Any().ShouldBeFalse();
            remote.All().EndpointsForAssembly("TestPackage2").Any().ShouldBeFalse();
        
        
            // Now, link in a new Bottle
            runBottles(@"
init src/TestPackage1 pak1
link harness pak1
");
            restart();

            remote.All().EndpointsForAssembly("TestPackage1").Any().ShouldBeTrue();
            remote.All().EndpointsForAssembly("TestPackage2").Any().ShouldBeFalse();

            // Link in a 2nd bottle
            runBottles(@"
link harness --clean-all
init src/TestPackage2 pak2
link harness pak2
");
            restart();

            remote.All().EndpointsForAssembly("TestPackage1").Any().ShouldBeFalse();
            remote.All().EndpointsForAssembly("TestPackage2").Any().ShouldBeTrue();

            // Link and unlink
            runBottles(@"
link harness --clean-all
init src/TestPackage1 pak1
link harness pak1
init-pak src/TestPackage2 pak2
link harness pak2
link harness pak1 --remove
");

            restart();

            remote.All().EndpointsForAssembly("TestPackage1").Any().ShouldBeFalse();
            remote.All().EndpointsForAssembly("TestPackage2").Any().ShouldBeTrue();


        }
    }
}