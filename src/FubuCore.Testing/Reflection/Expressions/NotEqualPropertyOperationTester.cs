using System;
using FubuCore.Reflection.Expressions;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Expressions
{
    public class NotEqualPropertyOperationTester {}

    [TestFixture]
    public class when_building_a_predicate_for_valueobject_not_equality
    {
        private Func<Contract, bool> _builtPredicate;

        [SetUp]
        public void Setup()
        {
            var builder = new NotEqualPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Contract>(c => c.Status)("Open").Compile();
        }

        [Test]
        public void should_not_succeed_when_the_property_contains_the_exact_value()
        {
            var contract = new Contract();
            contract.Status = "Open";
            _builtPredicate(contract).ShouldBeFalse();
        }

        [Test]
        public void should_succeed_when_the_property_contains_a_different_value()
        {
            var contract = new Contract();
            contract.Status = "Closed";
            _builtPredicate(contract).ShouldBeTrue();
        }
    }
}