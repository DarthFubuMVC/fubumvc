using System;
using FubuCore.Reflection.Expressions;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Expressions
{
    [TestFixture]
    public class when_building_a_predicate_for_string_ends_with
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
            var builder = new StringEndsWithPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Case>(c => c.Identifier)(input).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_ends_with_the_string_with_exact_case()
        {
            _builtPredicate(caseIdentifiedBy("barred")).ShouldBeTrue();
        }

        [Test]
        public void should_succeed_despite_different_casing()
        {
            _builtPredicate(caseIdentifiedBy("The last ReD")).ShouldBeTrue();
        }

        [Test]
        public void should_not_succeed_if_the_property_does_not_end_with_the_given_string()
        {
            _builtPredicate(caseIdentifiedBy("Red moon")).ShouldBeFalse();
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
    public class when_building_a_predicate_for_string_does_not_end_with
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
            var builder = new StringDoesNotEndWithPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Case>(c => c.Identifier)(input).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_ends_with_the_string_with_exact_case()
        {
            _builtPredicate(caseIdentifiedBy("barred")).ShouldBeFalse();
        }

        [Test]
        public void should_succeed_despite_different_casing()
        {
            _builtPredicate(caseIdentifiedBy("The last ReD")).ShouldBeFalse();
        }

        [Test]
        public void should_not_succeed_if_the_property_does_not_end_with_the_given_string()
        {
            _builtPredicate(caseIdentifiedBy("Red moon")).ShouldBeTrue();
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