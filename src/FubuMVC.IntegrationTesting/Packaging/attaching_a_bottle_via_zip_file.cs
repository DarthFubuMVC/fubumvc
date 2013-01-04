using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using FubuMVC.Core.Runtime;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;
using TestPackage1;
using FubuCore;

namespace FubuMVC.IntegrationTesting.Packaging
{
    [TestFixture, Ignore("passes, but it's too unreliable in CI")]
    public class attaching_a_bottle_via_zip_file : FubuRegistryHarness
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
        public void load_actions_from_a_bottle()
        {
            IEnumerable<string> names = remote.All().EndpointsForAssembly("TestPackage1").Select(x => x.FirstActionDescription);

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

            names.OrderBy(x => x)
                .ShouldHaveTheSameElementsAs(expectation);
        }

        [Test]
        public void can_invoke_a_json_endpoint_in_a_package()
        {
            // This endpoint is in the TestPackage1 package
            endpoints.PostJson(new JsonSerializedMessage
            {
                Name = "Jeremy"
            })
                .ReadAsJson<JsonSerializedMessage>()
                .Name.ShouldEqual("Jeremy");
        }

        [Test]
        public void can_invoke_string_endpoint_from_a_package()
        {
            endpoints.Get<TestPackage1.StringController>(x => x.SayHello())
                .ReadAsText()
                .ShouldEqual("Hello");
        }
    }
}