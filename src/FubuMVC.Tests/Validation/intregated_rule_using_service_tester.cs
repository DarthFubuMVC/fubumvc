using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    
    public class intregated_rule_using_service_tester
    {
        private CreateUser theModel;
        private IUserService theService;


        public intregated_rule_using_service_tester()
        {
            theModel = new CreateUser { Username = "test" };
            theService = MockRepository.GenerateStub<IUserService>();
        }

        private Notification theNotification
        {
            get
            {
                var scenario = ValidationScenario<CreateUser>.For(x =>
                {
                    x.Model = theModel;
                    x.Service(theService);
                    x.FieldRule(new UniqueUserNameRule());
                });

                return scenario.Notification;
            }
        }

        [Fact]
        public void fails_if_external_service_reports_failure()
        {
            theService.Stub(service => service.ExistsByUsername(theModel.Username)).Return(true);
            theNotification.MessagesFor<CreateUser>(m => m.Username).ShouldHaveCount(1);
        }

        [Fact]
        public void succeeds_if_external_service_reports_success()
        {
            theService.Stub(service => service.ExistsByUsername(theModel.Username)).Return(false);
            theNotification.MessagesFor<CreateUser>(m => m.Username).ShouldHaveCount(0);
        }

        public class CreateUser
        {
            public string Username { get; set; }
        }

        public interface IUserService
        {
            bool ExistsByUsername(string username);
        }

        public class UniqueUserNameRule : IFieldValidationRule
        {
            public static readonly string FIELD = "field";

	        public StringToken Token { get; set; }

			public ValidationMode Mode { get; set; }

	        public void Validate(Accessor accessor, ValidationContext context)
            {
                var username = context.GetFieldValue<string>(accessor);
                var userService = context.Service<IUserService>();
                
                if (userService.ExistsByUsername(username))
                {
                    var token = new ValidationKeys("Username {0} is already in use.".ToFormat(FIELD.AsTemplateField()));
                    var msg = new NotificationMessage(token);
                    msg.AddSubstitution(FIELD, username);

                    context.Notification.RegisterMessage(accessor, msg);
                }
            }
        }
    }
}