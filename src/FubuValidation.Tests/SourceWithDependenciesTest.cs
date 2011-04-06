using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class SourceWithDependenciesTest : InteractionContext<Validator> // piggy-backing on interactioncontext for container setup
    {
        protected override void beforeEach()
        {
            Container.Configure(x =>
                                    {
                                        x.For<ITypeResolver>().Use<TypeResolver>();
                                        x.For<IValidationSource>().Add(ctx => ctx.GetInstance<UniquePropertySource>());
                                    });
        }

        [Test]
        public void should_fail_if_external_service_reports_failure()
        {
            var model = new ModelWithUniqueProperty { Username = "test"};
            
            MockFor<IUserService>()
                .Expect(service => service.ExistsByUsername(model.Username))
                .Return(true);

            var notification = ClassUnderTest.Validate(model);
            notification
                .MessagesFor<ModelWithUniqueProperty>(m => m.Username)
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_succeed_if_external_service_reports_success()
        {
            var model = new ModelWithUniqueProperty { Username = "test" };

            MockFor<IUserService>()
                .Expect(service => service.ExistsByUsername(model.Username))
                .Return(false);

            var notification = ClassUnderTest.Validate(model);
            notification
                .MessagesFor<ModelWithUniqueProperty>(m => m.Username)
                .ShouldHaveCount(0);
        }
    }

    public class UniquePropertySource : IValidationSource
    {
        private readonly IContainer _container;

        public UniquePropertySource(IContainer container)
        {
            _container = container;
        }

        public IEnumerable<IValidationRule> RulesFor(Type type)
        {
            var rules = new List<IValidationRule>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            properties
                .Where(property => property.HasAttribute<UniqueAttribute>())
                .Each(property =>
                          {
                              Accessor accessor = new SingleProperty(property);
                              var rule = _container
                                          .With(accessor)
                                          .GetInstance<UsernameIsUniqueValueRule>();

                              rules.Add(rule);
                          });
            return rules;
        }
    }

    public interface IUserService
    {
        bool ExistsByUsername(string username);
    }

    public class UsernameIsUniqueValueRule : IValidationRule
    {
        private readonly Accessor _accessor;
        private readonly IUserService _userService;
        public static readonly string FIELD = "field";

        public UsernameIsUniqueValueRule(Accessor accessor, IUserService userService)
        {
            _accessor = accessor;
            _userService = userService;
        }

        public void Validate(ValidationContext context)
        {
            var value = (string)_accessor.GetValue(context.Target);
            if(_userService.ExistsByUsername(value))
            {
                var token = new ValidationKeys("UNIQUE_USER", "Username {0} is already in use.".ToFormat(FIELD.AsTemplateField()));
                var msg = new NotificationMessage(token);
                msg.AddSubstitution(FIELD, value);

                context.Notification.RegisterMessage(_accessor, msg);
            }
        }
    }

    public class ModelWithUniqueProperty
    {
        [Unique]
        public string Username { get; set; }
    }

    public class UniqueAttribute : Attribute
    {
    }
}