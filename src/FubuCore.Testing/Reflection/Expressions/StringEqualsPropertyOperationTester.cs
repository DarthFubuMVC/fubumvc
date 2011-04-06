using System;
using FubuCore.Reflection.Expressions;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Expressions
{
    [TestFixture]
    public class when_building_a_predicate_for_string_equals
    {
        private Func<Case, bool> _builtPredicate;

        private Case caseIdentifiedBy(string identifier)
        {
            return new Case
            {
                Identifier = identifier
            };
        }

        [SetUp]
        public void Setup()
        {
            const string input = "red";
            var builder = new StringEqualsPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Case>(c => c.Identifier)(input).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_contains_the_exact_string()
        {
            _builtPredicate(caseIdentifiedBy("red")).ShouldBeTrue();
        }

        [Test]
        public void should_succeed_despite_different_casing()
        {
            _builtPredicate(caseIdentifiedBy("ReD")).ShouldBeTrue();
        }

        [Test]
        public void should_not_succeed_if_the_given_string_is_not_the_same()
        {
            _builtPredicate(caseIdentifiedBy("RedEye")).ShouldBeFalse();
        }

        [Test]
        public void should_not_succeed_if_the_property_is_null()
        {
            var theCase = caseIdentifiedBy("RedEye");
            theCase.Identifier = null;
            _builtPredicate(theCase).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_building_a_predicate_for_string_does_not_equal
    {
        private Func<Case, bool> _builtPredicate;

        private Case caseIdentifiedBy(string identifier)
        {
            return new Case
            {
                Identifier = identifier
            };
        }

        [SetUp]
        public void Setup()
        {
            const string input = "red";
            var builder = new StringNotEqualPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Case>(c => c.Identifier)(input).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_contains_the_exact_string()
        {
            _builtPredicate(caseIdentifiedBy("red")).ShouldBeFalse();
        }

        [Test]
        public void should_succeed_despite_different_casing()
        {
            _builtPredicate(caseIdentifiedBy("ReD")).ShouldBeFalse();
        }

        [Test]
        public void should_not_succeed_if_the_given_string_is_not_the_same()
        {
            _builtPredicate(caseIdentifiedBy("RedEye")).ShouldBeTrue();
        }

        [Test]
        public void should_not_succeed_if_the_property_is_null()
        {
            var theCase = caseIdentifiedBy("RedEye");
            theCase.Identifier = null;
            _builtPredicate(theCase).ShouldBeTrue();
        }
    }

}