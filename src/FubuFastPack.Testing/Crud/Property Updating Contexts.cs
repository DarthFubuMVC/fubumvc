using System;
using System.Reflection;
using FubuFastPack.Crud.Properties;
using FubuFastPack.Persistence;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Testing.Security;
using FubuFastPack.Validation;
using FubuMVC.Core.UI.Security;
using FubuTestingSupport;
using FubuValidation;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace FubuFastPack.Testing.Crud
{


    [TestFixture]
    public class while_trying_to_update_a_property_without_authorization_rights : InteractionContext<PropertyUpdater<Part>>
    {
        private UpdatePropertyResultViewModel result;
        private Part thePart;

        protected override void beforeEach()
        {
            thePart = new Part{
                Name = "something"
            };

            Services.Inject<ISimplePropertyHandler<Part>>(Services.Container.GetInstance<SimplePropertyHandler<Part>>());
            var theProperty = ReflectionHelper.GetProperty<Part>(c => c.WarrantyDays);
            MockFor<IFieldAccessService>().Stub(x => x.RightsFor(thePart, theProperty)).Return(AccessRight.ReadOnly).Constraints(Is.Equal(thePart), Is.Matching<PropertyInfo>(x => x.Name == theProperty.Name));

            var update = new UpdatePropertyModel<Part>()
            {
                Id = Guid.NewGuid(),
                PropertyName = "WarrantyDays",
                PropertyValue = "abc",
            };

            MockFor<IRepository>().Stub(x => x.Find<Part>(update.Id)).Return(thePart);

            result = ClassUnderTest.EditProperty(update);
        }

        [Test]
        public void should_have_interrogated_the_user_rights_for_a_permission()
        {
            MockFor<IFieldAccessService>().VerifyAllExpectations();
        }

        [Test]
        public void should_return_success_false()
        {
            result.success.ShouldBeFalse();
        }

        [Test]
        public void should_show_not_authorized_as_the_message()
        {
            result.message.ShouldEqual(FastPackKeys.NOT_AUTHORIZED).ToString();
        }

        [Test]
        public void should_put_a_single_validation_message_in()
        {
            result.errors.Single().message.ShouldEqual(FastPackKeys.NOT_AUTHORIZED).ToString();
        }

    }

    // , UpdatePropertyModel<Part>, UpdatePropertyResultViewModel

    [TestFixture]
    public class while_updating_a_property_with_an_invalid_value : InteractionContext<PropertyUpdater<Part>>
    {
        private Part thePart;
        private UpdatePropertyModel<Part> theInput;
        private UpdatePropertyResultViewModel theOutput;


        protected override void beforeEach()
        {
            Services.Inject<IObjectConverter>(new ObjectConverter());
            Services.Inject<ISimplePropertyHandler<Part>>(Services.Container.GetInstance<SimplePropertyHandler<Part>>());
            var theProperty = ReflectionHelper.GetProperty<Part>(c => c.WarrantyDays);
            MockFor<IFieldAccessService>().Stub(x => x.RightsFor(typeof(Part), theProperty)).Return(AccessRight.All).IgnoreArguments();


            thePart = new Part();

            theInput = new UpdatePropertyModel<Part>()
            {
                Id = Guid.NewGuid(),
                PropertyName = "WarrantyDays",
                PropertyValue = "abc",
            };

            MockFor<IRepository>().Expect(x => x.Find<Part>(theInput.Id)).Return(thePart);
            MockFor<IValidator>().Expect(x => x.Validate(null)).Return(Notification.Valid()).IgnoreArguments();

            theOutput = ClassUnderTest.EditProperty(theInput);
        }

        [Test]
        public void the_success_should_be_false()
        {
            theOutput.success.ShouldBeFalse();
        }

        [Test]
        public void should_be_an_error()
        {
            theOutput.errors.Length.ShouldEqual(1);
        }
    }

    [TestFixture]
    public class while_recording_a_property_change_that_does_not_change_the_underlying_value : InteractionContext<PropertyUpdater<Case>>
    {
        private Case theCase;
        private UpdatePropertyModel<Case> Given;
        private UpdatePropertyResultViewModel Output;

        
        protected override void beforeEach()
        {
            Services.Inject<IObjectConverter>(new ObjectConverter());
            Services.Inject<ISimplePropertyHandler<Case>>(Services.Container.GetInstance<SimplePropertyHandler<Case>>());
            var theProperty = ReflectionHelper.GetProperty<Case>(c => c.Status);
            MockFor<IFieldAccessService>().Stub(x => x.RightsFor(typeof(Case), theProperty)).Return(AccessRight.All).IgnoreArguments();

            theCase = new Case() { Status = "Open" };

            Given = new UpdatePropertyModel<Case>()
            {
                Id = Guid.NewGuid(),
                PropertyName = "Status",
                PropertyValue = "Open",
            };

            MockFor<IRepository>().Expect(x => x.Find<Case>(Given.Id)).Return(theCase);
            MockFor<IValidator>().Expect(x => x.Validate(null)).Return(Notification.Valid()).IgnoreArguments();

            Output = ClassUnderTest.EditProperty(Given);
        }

        [Test]
        public void does_not_change_the_case()
        {
            MockFor<IRepository>().AssertWasNotCalled(x => x.Save(theCase));
        }

        [Test]
        public void do_NOT_create_a_property_log()
        {
            Output.ShouldNotBeNull();
            MockFor<IPropertyUpdateLogger<Case>>().AssertWasNotCalled(x => x.Log(theCase, null), x => x.IgnoreArguments());
        }

        [Test]
        public void the_success_of_the_return_value_to_the_client_should_be_success_equals_false_so_that_no_refresh_is_triggered()
        {
            Output.success.ShouldBeFalse();
        }
    }
}