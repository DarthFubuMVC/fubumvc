using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.View;
using Shouldly;
using HtmlTags;
using Xunit;

namespace FubuMVC.Tests.UI
{
    
    public class ReadOnlyIfNotAuthorizedTester
    {
        [Fact]
        public void do_nothing_to_a_div()
        {
            var tag = new HtmlTag("div").Authorized(false)
                .ReadOnlyIfNotAuthorized();

            tag.Authorized().ShouldBeFalse();
            tag.TagName().ShouldBe("div");
            
        }

        [Fact]
        public void do_nothing_to_a_link_that_is_authorized()
        {
            var tag = new HtmlTag("a").Authorized(true)
                .ReadOnlyIfNotAuthorized();

            tag.Authorized().ShouldBeTrue();
            tag.TagName().ShouldBe("a");
        }

        [Fact]
        public void change_an_unauthorized_link_to_a_span_and_authorize_the_span()
        {
            var tag = new HtmlTag("a").Authorized(false)
                .ReadOnlyIfNotAuthorized();

            tag.Authorized().ShouldBeTrue();
            tag.TagName().ShouldBe("span");
        }
    }

    
    public class RequiresAccessToTester
    {
        public RequiresAccessToTester()
        {
            PrincipalRoles.SetCurrentRolesForTesting("a", "b");
        }

        [Fact]
        public void positive_case_starting_from_authorized()
        {
            new HtmlTag("a").Authorized(true).RequiresAccessTo("a", "b")
                .Authorized().ShouldBeTrue();
        }

        [Fact]
        public void positive_case_starting_from_not_authorized()
        {
            new HtmlTag("a").Authorized(false).RequiresAccessTo("a", "b")
                .Authorized().ShouldBeFalse();
        }

        [Fact]
        public void negative_case_starting_from_authorized()
        {
            new HtmlTag("a").Authorized(true).RequiresAccessTo("not a current role of this user")
                .Authorized().ShouldBeFalse();
        }

        [Fact]
        public void negative_case_starting_from_not_authorized()
        {
            new HtmlTag("a").Authorized(false).RequiresAccessTo("not a current role of this user")
                .Authorized().ShouldBeFalse();
        }
    }

    
    public class ReadOnlyTester
    {
        [Fact]
        public void sets_the_disabled_attribute()
        {
            new HtmlTag("a").ReadOnly()
                .Attr("disabled").ShouldBe("disabled");
        }

        [Fact]
        public void set_the_disabled_attribute_if_the_condition_is_true()
        {
            new HtmlTag("a").ReadOnly(true)
                .Attr("disabled").ShouldBe("disabled");
        }

        [Fact]
        public void do_not_set_the_disabled_attribute_if_the_condition_is_not_true()
        {
            new HtmlTag("a").ReadOnly(false).HasAttr("disabled").ShouldBeFalse();
        }
    }
}