using System;
using FubuCore.Reflection.Expressions;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Expressions
{
    public class EqualsPropertyOperationTester{}

    public class Contract
    {
        public Contract()
        {
            Part = new Part();
        }

        public string Status { get; set; }
        public int Purchased { get; set; }
        public bool IsUnitBased { get; set; }

        public static Contract For(string status)
        {
            return new Contract(){
                Status = status
            };
        }

        public Part Part { get; set; }
    }


    public class Part
    {
        public bool IsUsed { get; set; }
    }

    [TestFixture]
    public class when_building_a_predicate_for_valueobject_equality
    {
        private Func<Contract, bool> _builtPredicate;

        [SetUp]
        public void Setup()
        {
            var builder = new EqualsPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Contract>(c => c.Status)("Open").Compile();
        }

        [Test]
        public void should_succeed_when_the_property_contains_the_exact_value()
        {
            var contract = Contract.For("Open");
            _builtPredicate(contract).ShouldBeTrue();
        }

        [Test]
        public void should_not_succeed_when_the_property_contains_a_different_value()
        {
            var contract = Contract.For("Closed");
            _builtPredicate(contract).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_building_a_predicate_for_numeric_equality
    {
        private Func<Contract, bool> _builtPredicate;

        [SetUp]
        public void Setup()
        {
            var builder = new EqualsPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Contract>(c => c.Purchased)(18).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_contains_the_exact_value()
        {
            var contract = new Contract();
            contract.Purchased = 18;
            _builtPredicate(contract).ShouldBeTrue();
        }

        [Test]
        public void should_not_succeed_when_the_property_contains_a_different_value()
        {
            var contract = new Contract();
            contract.Purchased = 19;
            _builtPredicate(contract).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_building_a_predicate_for_bool_expected_to_be_true
    {
        private Func<Contract, bool> _builtPredicate;

        [SetUp]
        public void Setup()
        {
            var builder = new EqualsPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Contract>(c => c.IsUnitBased)(true).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_is_true()
        {
            var contract = new Contract();
            contract.IsUnitBased = true;
            _builtPredicate(contract).ShouldBeTrue();
        }

        [Test]
        public void should_not_succeed_when_the_property_is_not_true()
        {
            var contract = new Contract();
            contract.IsUnitBased = false;
            _builtPredicate(contract).ShouldBeFalse();
        }
    }




    [TestFixture]
    public class when_building_a_predicate_for_bool_expected_to_be_false
    {
        private Func<Contract, bool> _builtPredicate;

        [SetUp]
        public void Setup()
        {
            var builder = new EqualsPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Contract>(c => c.IsUnitBased)(false).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_is_not_true()
        {
            var contract = new Contract();
            contract.IsUnitBased = false;
            _builtPredicate(contract).ShouldBeTrue();
        }

        [Test]
        public void should_not_succeed_when_the_property_is_true()
        {
            var contract = new Contract();
            contract.IsUnitBased = true;
            _builtPredicate(contract).ShouldBeFalse();
        }
    }

}