using System;
using System.Linq.Expressions;
using FubuCore;
using FubuFastPack.StructureMap;
using FubuFastPack.Validation;
using FubuTestApplication;
using FubuTestApplication.Domain;
using FubuTestingSupport;
using FubuValidation;
using NHibernate;
using NUnit.Framework;
using StructureMap;
using System.Linq;
using FubuCore.Reflection;

namespace IntegrationTesting.FubuFastPack.Validation
{
    [TestFixture]
    public class UniqueValidationIntegrationRuleTester
    {
        [SetUp]
        public void SetUp()
        {
            DatabaseDriver.Bootstrap(true);
           
        }

        [Test]
        public void validate_negative_case()
        {
            using (var container = DatabaseDriver.GetFullFastPackContainer().GetNestedContainer())
            {
                container.Configure(x => x.UseOnDemandNHibernateTransactionBoundary());

                var rule = container.UniqueRuleForProperties<Site>(x => x.Name);
                var context = container.ValidationContextFor(new Site{
                    Name = "something"
                });
                rule.Validate(context);

                context.Notification.IsValid().ShouldBeTrue();
            }

        }

        [Test]
        public void validate_positive_case_with_one_property()
        {
            DatabaseDriver.GetFullFastPackContainer().ExecuteInTransaction<IContainer>(c =>
            {
                var session = c.GetInstance<ISession>();
                session.Save(new Site { Name = "something" });
                session.Flush();

                var rule = c.UniqueRuleForProperties<Site>(x => x.Name);
                var context = c.ValidationContextFor(new Site
                {
                    Name = "something"
                });
                rule.ValidateAgainstSession(session, context);

                context.Notification.MessagesFor<Site>(x => x.Name).Single()
                    .StringToken.ShouldEqual(FastPackKeys.FIELD_MUST_BE_UNIQUE);
            });



        }


        [Test]
        public void validate_positive_case_with_two_properties()
        {
            DatabaseDriver.GetFullFastPackContainer().ExecuteInTransaction<IContainer>(c =>
            {
                var session = c.GetInstance<ISession>();
                session.Save(new Case{
                    Identifier = "something",
                    CaseType = "Open"
                });
                session.Flush();

                var rule = c.UniqueRuleForProperties<Case>(x => x.Identifier, x => x.CaseType);
                var context = c.ValidationContextFor(new Case{
                    Identifier = "something",
                    CaseType = "Open"
                });

                rule.ValidateAgainstSession(session, context);

                context.Notification.MessagesFor<Case>(x => x.Identifier).Single()
                    .StringToken.ShouldEqual(FastPackKeys.FIELD_MUST_BE_UNIQUE);

                context.Notification.MessagesFor<Case>(x => x.CaseType).Single()
                    .StringToken.ShouldEqual(FastPackKeys.FIELD_MUST_BE_UNIQUE);
            });
        }

        [Test]
        public void validate_negative_case_with_two_properties()
        {
            DatabaseDriver.GetFullFastPackContainer().ExecuteInTransaction<IContainer>(c =>
            {
                var session = c.GetInstance<ISession>();
                session.Save(new Case { Identifier = "something", CaseType = "Open" });
                session.Flush();

                var rule = c.UniqueRuleForProperties<Case>(x => x.Identifier, x => x.CaseType);
                var context = c.ValidationContextFor(new Case
                {
                    Identifier = "something",
                    CaseType = "different"
                });

                rule.ValidateAgainstSession(session, context);

                context.Notification.IsValid().ShouldBeTrue();

            });



        }


    }

    public static class UniqueValidationExtensions
    {
        public static UniqueValidationRule UniqueRuleForProperties<T>(this IContainer container, params Expression<Func<T, object>>[] properties)
        {
            var processor = container.GetInstance<ITransactionProcessor>();
            return new UniqueValidationRule("whatever", typeof(T), processor, properties.Select(x => x.ToAccessor().InnerProperty));
        }

        public static ValidationContext ValidationContextFor(this IContainer container, object target)
        {
            return new ValidationContext(container.GetInstance<IValidator>(), new Notification(target.GetType()), target){
                Resolver = container.GetInstance<ITypeResolver>(),
                TargetType = target.GetType()
            };
        }
    }
}