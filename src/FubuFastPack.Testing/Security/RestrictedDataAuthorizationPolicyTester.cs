using System;
using System.Linq.Expressions;
using FubuFastPack.Domain;
using FubuFastPack.Lists;
using FubuFastPack.Querying;
using FubuFastPack.Security;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuFastPack.Testing.Security
{
    [TestFixture]
    public class RestrictedDataAuthorizationPolicyTester : InteractionContext<RestrictedDataAuthorizationPolicy<Case>>
    {
        private Case _theCase;
        private IFubuRequest _fubuRequest;

        [Test]
        public void should_not_deny_access_if_none_of_the_data_restrictions_deny_it()
        {
            _theCase = new Case
            {
                Condition = "Open",
                Title = "The Title"
            };
            var dataRestrictions = new IDataRestriction<Case>[]
            {
                new CasePropertyRestriction(x => x.Condition, "Open"),
                new CasePropertyRestriction(x => x.Title, "The Title")
            };
            Services.InjectArray(dataRestrictions);
            _fubuRequest = MockFor<IFubuRequest>();
            _fubuRequest.Stub(x => x.Get<Case>()).Return(_theCase);
            ClassUnderTest.RightsFor(_fubuRequest).ShouldNotEqual(AuthorizationRight.Deny);
        }

        [Test]
        public void should_deny_access_if_any_of_the_data_restrictions_deny_it()
        {
            _theCase = new Case
            {
                Condition = "Open",
                Title = "A Different Title"
            };
            var dataRestrictions = new IDataRestriction<Case>[]
            {
                new CasePropertyRestriction(x => x.Condition, "Open"),
                new CasePropertyRestriction(x => x.Title, "The Title") // does not match, should deny
            };
            Services.InjectArray(dataRestrictions);
            _fubuRequest = MockFor<IFubuRequest>();
            _fubuRequest.Stub(x => x.Get<Case>()).Return(_theCase);
            ClassUnderTest.RightsFor(_fubuRequest).ShouldEqual(AuthorizationRight.Deny);
        }
    }

    class CasePropertyRestriction : IDataRestriction<Case>
    {
        private readonly object _allowedValue;
        private Expression<Func<Case, object>> _propertyExpression;

        public CasePropertyRestriction(Expression<Func<Case, object>> propertyExpression, object allowedValue)
        {
            _propertyExpression = propertyExpression;
            _allowedValue = allowedValue;
        }

        public void Apply(IDataSourceFilter<Case> filter)
        {
            filter.WhereEqual(_propertyExpression, _allowedValue);
        }
    }

    public class User : DomainEntity
    {
        public string Username { get; set; }
    }

    public class Case : DomainEntity
    {
        public string Condition { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }
        public bool IsSecret { get; set; }
        public Person Owner { get; set; }

        public int Integer { get; set; }

        [ListValue("CaseType")]
        public string CaseType { get; set; }

        public string Status { get; set; }
    }

    public class Site : DomainEntity{}
    public class Solution : DomainEntity {}

    public class Person :DomainEntity
    {
        public string Name { get; set; }
    }

    public static class DomainEntityExtensions
    {
        public static T WithId<T>(this T entity) where T :DomainEntity
        {
            entity.Id = Guid.NewGuid();
            return entity;
        }
    }

    public class Part : DomainEntity
    {
        public string Name { get; set; }

        public int WarrantyDays { get; set; }
    }
}