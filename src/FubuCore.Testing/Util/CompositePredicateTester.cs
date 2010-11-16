using FubuCore.Util;
using NUnit.Framework;

namespace FubuCore.Testing.Util
{
    [TestFixture]
    public class CompositeFilterTester
    {
        [Test]
        public void has_changed()
        {
            var filter = new CompositeFilter<string>();
            filter.HasChanged.ShouldBeFalse();
            filter.Includes += x => x == "a";

            filter.HasChanged.ShouldBeTrue();

            filter.ResetChangeTracking();
            filter.HasChanged.ShouldBeFalse();

            filter.Excludes += x => x == "a";
            filter.HasChanged.ShouldBeTrue();

            filter.ResetChangeTracking();
            filter.HasChanged.ShouldBeFalse();
        }

        [Test]
        public void match_with_includes_and_no_excludes()
        {
            var filter = new CompositeFilter<string>();
            filter.Includes += x => x == "a";

            filter.Matches("a").ShouldBeTrue();
            filter.Matches("b").ShouldBeFalse();
        }

        [Test]
        public void match_with_matching_include_and_not_matching_exclude()
        {
            var filter = new CompositeFilter<string>();
            filter.Includes += x => x.StartsWith("a");
            filter.Excludes += x => x == "a";

            filter.Matches("a").ShouldBeFalse();
            filter.Matches("abc").ShouldBeTrue();
            filter.Matches("b").ShouldBeFalse();
        }

        [Test]
        public void match_with_no_includes_an_no_excludes()
        {
            var filter = new CompositeFilter<string>();
            filter.Excludes.DoesNotMatchAny("a").ShouldBeTrue();
            filter.Matches("a").ShouldBeTrue();
        }
    }

    [TestFixture]
    public class CompositePredicateTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void matches_none()
        {
            var composite = new CompositePredicate<string>();

            composite.MatchesNone("a").ShouldBeFalse();

            composite += x => x == "b";

            composite.MatchesNone("a").ShouldBeTrue();

            composite += x => x == "a";


            composite.MatchesNone("a").ShouldBeFalse();
        }

        [Test]
        public void no_predicates_means_that_it_always_true()
        {
            var composite = new CompositePredicate<string>();
            composite.MatchesAll("a").ShouldBeTrue();
            composite.MatchesAll("b").ShouldBeTrue();
            composite.MatchesAll("c").ShouldBeTrue();
            composite.MatchesAll("d").ShouldBeTrue();
            composite.MatchesAll("e").ShouldBeTrue();
            composite.MatchesAll("f").ShouldBeTrue();
            composite.MatchesAll("g").ShouldBeTrue();
        }

        [Test]
        public void or_testing()
        {
            var composite = new CompositePredicate<string>();

            composite.MatchesAny("a").ShouldBeTrue();

            composite += x => x == "b";

            composite.MatchesAny("a").ShouldBeFalse();

            composite += x => x.StartsWith("a");

            composite.MatchesAny("a").ShouldBeTrue();
        }

        [Test]
        public void registered_predicates_are_used()
        {
            var composite = new CompositePredicate<string>();
            composite.MatchesAll("a").ShouldBeTrue();
            composite.MatchesAll("b").ShouldBeTrue();

            composite += x => x == "a";

            composite.MatchesAll("a").ShouldBeTrue();
            composite.MatchesAll("b").ShouldBeFalse();
        }

        [Test]
        public void use_multiple_predicates()
        {
            var composite = new CompositePredicate<string>();
            composite += x => x.StartsWith("a");
            composite += x => x.EndsWith("x");

            composite.MatchesAll("a").ShouldBeFalse();
            composite.MatchesAll("x").ShouldBeFalse();
            composite.MatchesAll("abx").ShouldBeTrue();
        }
    }
}