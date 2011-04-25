using Bottles.Deployment.Commands;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Commands
{
    [TestFixture]
    public class AddDirectiveCommandTester : InteractionContext<AddDirectiveCommand>
    {
        private AddDirectiveInput theInput;
        
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


        private void finds_directive()
        {
            MockFor<IDirectiveTypeRegistry>().Stub(x => x.DirectiveTypeFor("FubuWebsite")).Return(typeof (FubuWebsite));
        }
        private void execute()
        {
            ClassUnderTest.Initialize(MockFor<IDirectiveTypeRegistry>(), theInput);
        }

        [Test]
        public void should_select_the_correct_directive()
        {
            finds_directive();

            execute();

            MockFor<IDirectiveTypeRegistry>().AssertWasCalled(x=>x.DirectiveTypeFor("FubuWebsite"));
        }




    }
}
