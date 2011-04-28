using Bottles.Configuration;
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
        private string profile = @".\refCommand";
        private string bottleToAdd = "bob";
        private string recipe = "recipe2";

        private void setupHostManifest()
        {
            var profileWriter = new DeploymentWriter(profile);
            profileWriter.RecipeFor(recipe).HostFor(hostName).AddDirective(new SimpleSettings());
            profileWriter.Flush(FlushOptions.Wipeout);
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
                            DeploymentFlag = profile
                        };

            MockFor<IProfileFinder>().Stub(pf => pf.FindDeploymentFolder(@".\refCommand")).Return(@".\refCommand");

            ClassUnderTest.Exe(input, new EnvironmentSettings(), MockFor<IFileSystem>(), MockFor<IProfileFinder>());
            
            MockFor<IFileSystem>().AssertWasCalled(fs=>fs.AppendStringToFile(@".\refCommand\recipes\recipe2\web.host","bottle:bob "));
        }


        

    }
}