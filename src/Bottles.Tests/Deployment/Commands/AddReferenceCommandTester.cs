using Bottles.Configuration;
using Bottles.Deployment;
using Bottles.Deployment.Commands;
using Bottles.Deployment.Writing;
using Bottles.Tests.Deployment.Writing;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Deployment.Commands
{
    [TestFixture]
    public class AddReferenceCommandTester : InteractionContext<AddReferenceCommand>
    {
        private string hostName = "web";
        private string deployment = @".\refCommand";
        private string bottleToAdd = "bob";
        private string recipe = "recipe2";

        private void setupHostManifest()
        {
            var deploymentWriter = new DeploymentWriter(deployment);
            deploymentWriter.RecipeFor(recipe).HostFor(hostName).AddDirective(new SimpleSettings());

            deploymentWriter.Flush(FlushOptions.Wipeout);
        }

        [Test]
        public void Name()
        {
            setupHostManifest();

            var input = new AddReferenceCommandInput
                        {
                            Recipe = recipe,
                            Host = hostName,
                            Bottle = bottleToAdd,
                            RelationshipFlag = null,
                            DeploymentFlag = deployment
                        };

            MockFor<IProfileFinder>().Stub(pf => pf.FindDeploymentFolder(@".\refCommand")).Return(@".\refCommand");

            ClassUnderTest.Exe(input, new EnvironmentSettings(), MockFor<IFileSystem>(), new DeploymentSettings(@".\refCommand"));
            
            MockFor<IFileSystem>().AssertWasCalled(fs=>fs.AppendStringToFile(@".\refCommand\recipes\recipe2\web.host","bottle:bob "));
        }


        

    }
}