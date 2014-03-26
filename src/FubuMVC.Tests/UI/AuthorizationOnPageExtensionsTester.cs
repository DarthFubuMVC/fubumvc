using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class ReadOnlyIfNotAuthorizedTester
    {
        [Test]
        public void do_nothing_to_a_div()
        {
            var tag = new HtmlTag("div").Authorized(false)
                .ReadOnlyIfNotAuthorized();

            tag.Authorized().ShouldBeFalse();
            tag.TagName().ShouldEqual("div");
            
        }

        [Test]
        public void do_nothing_to_a_link_that_is_authorized()
        {
            var tag = new HtmlTag("a").Authorized(true)
                .ReadOnlyIfNotAuthorized();

            tag.Authorized().ShouldBeTrue();
            tag.TagName().ShouldEqual("a");
        }

        [Test]
        public void change_an_unauthorized_link_to_a_span_and_authorize_the_span()
        {
            var tag = new HtmlTag("a").Authorized(false)
                .ReadOnlyIfNotAuthorized();

            tag.Authorized().ShouldBeTrue();
            tag.TagName().ShouldEqual("span");
        }
    }

    [TestFixture]
    public class RequiresAccessToTester
    {
        [SetUp]
        public void SetUp()
        {
            PrincipalRoles.SetCurrentRolesForTesting("a", "b");
        }

        [Test]
        public void positive_case_starting_from_authorized()
        {
            new HtmlTag("a").Authorized(true).RequiresAccessTo("a", "b")
                .Authorized().ShouldBeTrue();
        }

        [Test]
        public void positive_case_starting_from_not_authorized()
        {
            new HtmlTag("a").Authorized(false).RequiresAccessTo("a", "b")
                .Authorized().ShouldBeFalse();
        }

        [Test]
        public void negative_case_starting_from_authorized()
        {
            new HtmlTag("a").Authorized(true).RequiresAccessTo("not a current role of this user")
                .Authorized().ShouldBeFalse();
        }

        [Test]
        public void negative_case_starting_from_not_authorized()
        {
            new HtmlTag("a").Authorized(false).RequiresAccessTo("not a current role of this user")
                .Authorized().ShouldBeFalse();
        }
    }

    [TestFixture]
    public class ReadOnlyTester
    {
        [Test]
        public void sets_the_disabled_attribute()
        {
            new HtmlTag("a").ReadOnly()
                .Attr("disabled").ShouldEqual("disabled");
        }

        [Test]
        public void set_the_disabled_attribute_if_the_condition_is_true()
        {
            new HtmlTag("a").ReadOnly(true)
                .Attr("disabled").ShouldEqual("disabled");
        }

        [Test]
        public void do_not_set_the_disabled_attribute_if_the_condition_is_not_true()
        {
            new HtmlTag("a").ReadOnly(false).HasAttr("disabled").ShouldBeFalse();
        }
    }
}