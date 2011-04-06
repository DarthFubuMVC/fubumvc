using System;
using FubuCore.Reflection.Expressions;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Expressions
{
    [TestFixture]
    public class when_building_a_predicate_for_string_contains
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
            var builder = new StringContainsPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Case>(c => c.Identifier)(input).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_contains_the_string_in_the_exact_case()
        {
            _builtPredicate(caseIdentifiedBy("the red wings")).ShouldBeTrue();
        }

        [Test]
        public void should_succeed_despite_different_casing()
        {
            _builtPredicate(caseIdentifiedBy("the RED scare")).ShouldBeTrue();
        }

        [Test]
        public void should_not_succeed_if_the_given_string_is_not_found()
        {
            _builtPredicate(caseIdentifiedBy("the blue guys")).ShouldBeFalse();
        }

        [Test]
        public void should_not_succeed_if_the_property_is_null()
        {
            var theCase = caseIdentifiedBy("the blue guys");
            theCase.Identifier = null;
            _builtPredicate(theCase).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_building_a_predicate_for_string_does_not_contain
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
            var builder = new StringContainsPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Case>(c => c.Identifier)(input).Compile();
        }

        [Test]
        public void should_succeed_when_the_property_contains_the_string_in_the_exact_case()
        {
            _builtPredicate(caseIdentifiedBy("the red wings")).ShouldBeTrue();
        }

        [Test]
        public void should_succeed_despite_different_casing()
        {
            _builtPredicate(caseIdentifiedBy("the RED scare")).ShouldBeTrue();
        }

        [Test]
        public void should_not_succeed_if_the_given_string_is_not_found()
        {
            var theCase = caseIdentifiedBy("the blue guys");
            _builtPredicate(theCase).ShouldBeFalse();
        }

        [Test]
        public void should_not_succeed_if_the_property_is_null()
        {
            var theCase = caseIdentifiedBy("the blue guys");
            theCase.Identifier = null;
            _builtPredicate(theCase).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_building_a_predicate_for_string_contains_with_a_deep_property
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
            var builder = new StringContainsPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Case>(c => c.Contact.PrimarySite.PrimaryAddress.StateOrProvince)(input).Compile();
        }

        [Test]
        public void succeed_when_contains()
        {
            var theCase = caseIdentifiedBy("Ignored");
            theCase.Contact.PrimarySite.PrimaryAddress.StateOrProvince = "TredXAS";
            _builtPredicate(theCase).ShouldBeTrue();
        }

        [Test]
        public void fail_when_doesnt_contain()
        {
            var theCase = caseIdentifiedBy("Ignored");
            theCase.Contact.PrimarySite.PrimaryAddress.StateOrProvince = "Texas";
            _builtPredicate(theCase).ShouldBeFalse();
        }

        [Test]
        public void fail_when_the_property_is_null()
        {
            var theCase = caseIdentifiedBy("Ignored");
            theCase.Contact.PrimarySite.PrimaryAddress.StateOrProvince = null;
            _builtPredicate(theCase).ShouldBeFalse();
        }

    }

    [TestFixture]
    public class when_building_a_predicate_for_string_does_not_contain_with_a_deep_property
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
            var builder = new StringContainsPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Case>(c => c.Contact.PrimarySite.PrimaryAddress.StateOrProvince)(input).Compile();
        }

        [Test]
        public void succeed_when_contains()
        {
            var theCase = caseIdentifiedBy("Ignored");
            theCase.Contact.PrimarySite.PrimaryAddress.StateOrProvince = "TredXAS";
            _builtPredicate(theCase).ShouldBeTrue();
        }

        [Test]
        public void fail_when_doesnt_contain()
        {
            var theCase = caseIdentifiedBy("Ignored");
            theCase.Contact.PrimarySite.PrimaryAddress.StateOrProvince = "Texas";
            _builtPredicate(theCase).ShouldBeFalse();
        }

        [Test]
        public void fail_when_the_property_is_null()
        {
            var theCase = caseIdentifiedBy("Ignored");
            theCase.Contact.PrimarySite.PrimaryAddress.StateOrProvince = null;
            _builtPredicate(theCase).ShouldBeFalse();
        }

    }
}