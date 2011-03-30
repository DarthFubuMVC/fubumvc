using FubuFastPack.Querying;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.Querying
{
    [TestFixture]
    public class SingleEntityFilterTester
    {
        [Test]
        public void where_equal_will_allow_viewing_if_the_entitys_property_is_a_specific_value()
        {
            var theCase = new Case();
            theCase.Title = "The Title";

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereEqual(x => x.Title, theCase.Title);

            filter.CanView.ShouldBeTrue();
        }

        [Test]
        public void where_equal_will_allow_viewing_if_the_entitys_property_and_expected_value_is_null()
        {
            var theCase = new Case();
            theCase.Title = null;

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereEqual(x => x.Title, null);

            filter.CanView.ShouldBeTrue();
        }

        [Test]
        public void where_equal_should_handle_boxed_values()
        {
            var theCase = new Case();
            theCase.IsSecret = true;

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereEqual(x => x.IsSecret, true);

            filter.CanView.ShouldBeTrue();
        }

        [Test]
        public void where_not_equal_should_handle_boxed_values()
        {
            var theCase = new Case();
            theCase.IsSecret = true;

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereNotEqual(x => x.IsSecret, false);

            filter.CanView.ShouldBeTrue();
        }

        [Test]
        public void where_equal_should_handle_entities()
        {
            var person = new Person().WithId();
            var theCase = new Case().WithId();
            theCase.Owner = person;

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereEqual(x => x.Owner, person);

            filter.CanView.ShouldBeTrue();
        }

        [Test]
        public void where_not_equal_should_handle_entities()
        {
            var person = new Person().WithId();
            var otherPerson = new Person().WithId();
            var theCase = new Case().WithId();
            theCase.Owner = otherPerson;

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereNotEqual(x => x.Owner, person);

            filter.CanView.ShouldBeTrue();
        }

        [Test]
        public void where_equal_will_deny_viewing_if_the_entitys_property_is_not_a_specific_value()
        {
            var theCase = new Case();
            theCase.Title = "The Title";

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereEqual(x => x.Title, "Not the Title");

            filter.CanView.ShouldBeFalse();
        }

        [Test]
        public void where_not_equal_will_allow_viewing_if_the_entitys_property_is_not_a_specific_value()
        {
            var theCase = new Case();
            theCase.Title = "The Title";

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereNotEqual(x => x.Title, "Not the title");

            filter.CanView.ShouldBeTrue();
        }

        [Test]
        public void where_not_equal_will_deny_viewing_if_the_entitys_property_is_a_specific_value()
        {
            var theCase = new Case();
            theCase.Title = "The Title";

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereNotEqual(x => x.Title, theCase.Title);

            filter.CanView.ShouldBeFalse();
        }

        [Test]
        public void where_not_equal_will_deny_viewing_if_the_entitys_property_and_expected_value_is_null()
        {
            var theCase = new Case();
            theCase.Title = null;

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereNotEqual(x => x.Title, null);

            filter.CanView.ShouldBeFalse();
        }

        [Test]
        public void a_single_deny_among_many_filters_will_deny_the_entity()
        {
            var theCase = new Case();
            theCase.Title = "The Title";
            theCase.Condition = "Open";
            theCase.Reason = "Fun";

            var filter = new SingleEntityFilter<Case>(theCase);
            filter.WhereEqual(x => x.Condition, "Open");
            filter.WhereNotEqual(x => x.Title, theCase.Title);
            filter.WhereEqual(x => x.Reason, "Fun");

            filter.CanView.ShouldBeFalse();
        }

        [Test]
        public void denying_restriction_indicates_which_restriction_denied_the_entity()
        {
            var theCase = new Case();
            theCase.Title = "The Title";
            theCase.Condition = "Open";
            theCase.Reason = "Fun";

            var filter = new SingleEntityFilter<Case>(theCase);
            var conditionRestriction = new CasePropertyRestriction(x => x.Condition, "Open");
            var titleRestriction = new CasePropertyRestriction(x => x.Title, "Not the Title");
            var originRestriction = new CasePropertyRestriction(x => x.Reason, "Fun");

            filter.ApplyRestriction(conditionRestriction);
            filter.ApplyRestriction(titleRestriction);
            filter.ApplyRestriction(originRestriction);

            filter.DenyingRestriction.ShouldBeTheSameAs(titleRestriction);
        }

    }
}