using System;
using System.Linq;
using FubuMVC.Core;
using IntegrationTesting.Conneg;
using NUnit.Framework;
using FubuTestingSupport;

namespace IntegrationTesting.Packaging
{
    [TestFixture]
    public class loading_actions_from_a_linked_package : FubuRegistryHarness
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

        [Test]
        public void load_actions_from_a_bottle()
        {
            remote.All().EndpointsForAssembly("TestPackage1").Select(x => x.FirstActionDescription)
                .ShouldHaveTheSameElementsAs("StringController.SayHello()", "JsonController.SendMessage()", "ViewController.ShowView()");
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