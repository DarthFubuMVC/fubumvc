using FubuFastPack.Crud;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using FubuValidation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuFastPack.Testing.Crud
{
    [TestFixture]
    public class when_handling_a_valid_creation_request : InteractionContext<EntityCreator<EditCaseViewModel, Case>>
    {
        private CreationRequest<EditCaseViewModel> _input;
        private Case _theCase;
        private CrudReport _output;

        protected override void beforeEach()
        {
            _theCase = new Case();
            MockFor<IValidator>().Stub(x => x.Validate(_theCase)).Return(Notification.Valid());

            var editCaseViewModel = new EditCaseViewModel(_theCase);
            _input = new CreationRequest<EditCaseViewModel>(editCaseViewModel);
            _output = ClassUnderTest.Create(_input);
        }

        [Test]
        public void output_should_indicate_success()
        {
            _output.success.ShouldBeTrue();
        }


    }
}