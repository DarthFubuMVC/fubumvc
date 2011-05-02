using Bottles.Deployment;
using Bottles.Deployment.Commands;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using Bottles.Deployment.Writing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Commands
{
    [TestFixture]
    public class AddDirectiveCommandTester : InteractionContext<AddDirectiveCommand>
    {
        private AddDirectiveInput theInput;
        private DeploymentSettings theSettings;

        protected override void beforeEach()
        {

            theInput = new AddDirectiveInput()
                       {
                           Directive = "FubuWebsite",
                           Host = "midge",
                           Recipe = "brownies",
                           DeploymentFlag = @".\iisfubu"
                       };
        }

        private void setup_profile()
        {
            var pw = new DeploymentWriter(@".\iisfubu");
            var r = pw.RecipeFor("brownies");

            pw.Flush(FlushOptions.Wipeout);
        }
        private void finds_directive()
        {
            MockFor<IProfileFinder>().Stub(x => x.FindDeploymentFolder(@".\iisfubu")).Return(@".\iisfubu");
            MockFor<IDirectiveTypeRegistry>().Stub(x => x.DirectiveTypeFor("FubuWebsite")).Return(typeof (FubuWebsite));
        }
        private void execute()
        {
            theSettings = new DeploymentSettings(@".\iisfubu");
            ClassUnderTest.Initialize(MockFor<IDirectiveTypeRegistry>(), theInput, theSettings);
        }

        [Test]
        public void should_select_the_correct_directive()
        {
            setup_profile();

            finds_directive();

            execute();

            MockFor<IDirectiveTypeRegistry>().AssertWasCalled(x=>x.DirectiveTypeFor("FubuWebsite"));
        }




    }
}
