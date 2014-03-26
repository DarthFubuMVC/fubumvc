using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Security
{
    [TestFixture]
    public class FieldAccessServiceIntegratedTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            jNameRule = new JNameRule {Category = FieldAccessCategory.Authorization};
            var rules = new List<IFieldAccessRule>
            {
                jNameRule,
                new LimiterRule<PersonModel>(person => person.Age > 30, AccessRight.ReadOnly)
                {Category = FieldAccessCategory.LogicCondition}
            };

            var services = new InMemoryServiceLocator();
            services.Add(new JNameRule());

            _service = new FieldAccessService(new FieldAccessRightsExecutor(), rules, null, services);
        }

        #endregion

        private FieldAccessService _service;
        private JNameRule jNameRule;

        private AccessRight RightsFor<T>(T model, Expression<Func<T, object>> expression)
        {
            ElementRequest request = ElementRequest.For(model, expression);
            return _service.RightsFor(request);
        }

        [Test]
        public void puts_the_service_locator_on_the_request()
        {
            RightsFor(new PersonModel {Name = "Jeremy"}, x => x.Name);

            // measuring this against a side effect
            jNameRule.LastRequest.Get<JNameRule>().ShouldNotBeNull();
        }

        [Test]
        public void logic_rule_applies_but_no_security()
        {
            // Logic rule should limit anything over 30
            RightsFor(new PersonModel {Age = 31}, x => x.Age).ShouldEqual(AccessRight.ReadOnly);
            RightsFor(new PersonModel {Age = 29}, x => x.Age).ShouldEqual(AccessRight.All);
        }

        [Test]
        public void no_rules_of_any_kind_apply()
        {
            RightsFor(new TestInputModel(), x => x.Age).ShouldEqual(AccessRight.All);
        }

        [Test]
        public void security_rule_applies_but_no_logic()
        {
            RightsFor(new PlayerModel {Name = "Jeremy"}, x => x.Name).ShouldEqual(AccessRight.All);
            RightsFor(new PlayerModel {Name = "Chad"}, x => x.Name).ShouldEqual(AccessRight.ReadOnly);
        }
    }


    public class JNameRule : IFieldAccessRule
    {
        public ElementRequest LastRequest;

        #region IFieldAccessRule Members

        public AccessRight RightsFor(ElementRequest request)
        {
            LastRequest = request;
            if (request.Value<string>().StartsWith("J")) return AccessRight.All;

            

            return AccessRight.ReadOnly;
        }

        public bool Matches(Accessor accessor)
        {
            return accessor.Name == "Name";
        }

        public FieldAccessCategory Category { get; set; }

        #endregion
    }

    public class LimiterRule<T> : IFieldAccessRule
    {
        private readonly Func<T, bool> _filter;
        private readonly AccessRight _limitation;

        public LimiterRule(Func<T, bool> filter, AccessRight limitation)
        {
            _filter = filter;
            _limitation = limitation;
        }

        #region IFieldAccessRule Members

        public AccessRight RightsFor(ElementRequest request)
        {
            return _filter(request.Model.As<T>()) ? _limitation : AccessRight.All;
        }

        public bool Matches(Accessor accessor)
        {
            return accessor.OwnerType == typeof (T);
        }

        public FieldAccessCategory Category { get; set; }

        #endregion
    }

    public class PersonModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class PlayerModel
    {
        public string Name { get; set; }
        public string Position { get; set; }
    }

    public class TestInputModel
    {
        public int PropInt { get; set; }
        public string Prop1 { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}