using System;
using FubuCore.Reflection.Expressions;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Expressions
{
    public class Case
    {
        public Case()
        {
            Contact = new Contact();
        }

        public string Identifier { get; set; }
        public bool HasPart { get; set; }
        public Contact Contact { get; set; }
    }


    [TestFixture]
    public class when_building_a_predicate_for_string_starts_with
    {
        private Func<Case, bool> _builtPredicate;

        private Case caseIdentifiedBy(string identifier)
        {
            return new Case{
                Identifier = identifier
            };
        }

        [SetUp]
        public void Setup()
        {
            const string input = "red";
            var builder = new StringStartsWithPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Case>(c => c.Identifier)(input).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_starts_with_the_exact_string()
        {
            _builtPredicate(caseIdentifiedBy("red white and blue")).ShouldBeTrue();
        }

        [Test]
        public void should_succeed_despite_different_casing()
        {
            _builtPredicate(caseIdentifiedBy("ReD RuM")).ShouldBeTrue();
        }

        [Test]
        public void should_not_succeed_if_the_property_does_not_start_with_the_given_string()
        {
            _builtPredicate(caseIdentifiedBy("the red fern")).ShouldBeFalse();
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
    public class when_building_a_predicate_for_string_does_not_start_with
    {
        private Case caseIdentifiedBy(string identifier)
        {
            return new Case
            {
                Identifier = identifier
            };
        }

        private Func<Case, bool> _builtPredicate;

        [SetUp]
        public void Setup()
        {
            const string input = "red";
            var builder = new StringDoesNotStartWithPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Case>(c => c.Identifier)(input).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_starts_with_the_exact_string()
        {
            _builtPredicate(caseIdentifiedBy("red white and blue")).ShouldBeFalse();
        }

        [Test]
        public void should_succeed_despite_different_casing()
        {
            _builtPredicate(caseIdentifiedBy("ReD RuM")).ShouldBeFalse();
        }

        [Test]
        public void should_not_succeed_if_the_property_does_not_start_with_the_given_string()
        {
            _builtPredicate(caseIdentifiedBy("the red fern")).ShouldBeTrue();
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