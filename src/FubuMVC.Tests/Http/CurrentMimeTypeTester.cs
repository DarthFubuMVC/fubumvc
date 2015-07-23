using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class CurrentMimeTypeTester
    {
        [Test]
        public void default_is_to_accept_anything()
        {
            new CurrentMimeType()
                .AcceptTypes.Single()
                .ShouldBe(MimeType.Any.ToString());
        }

        [Test]
        public void feeding_in_null_for_contentType_defaults_to_HttpFormMimeType()
        {
            var currentMimeType = new CurrentMimeType(null, null);
            currentMimeType.ContentType.ShouldBe(MimeType.HttpFormMimetype);
        }

        [Test]
        public void is_smart_enough_to_pull_out_charset()
        {
            var currentMimeType = new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", null);
            currentMimeType.ContentType.ShouldBe("application/x-www-form-urlencoded");
            currentMimeType.Charset.ShouldBe("UTF-8");
        }

        [Test]
        public void accepts_html_negative()
        {
            var currentMimeType = new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", null);
            currentMimeType.AcceptsHtml().ShouldBeFalse();
        }

        [Test]
        public void accepts_html_positive()
        {
            new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", "text/html").AcceptsHtml().ShouldBeTrue();
            new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", "text/json, text/html").AcceptsHtml().ShouldBeTrue();
        }

        [Test]
        public void accepts_any_negative()
        {
            new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", "text/html").AcceptsAny().ShouldBeFalse();
        }

        [Test]
        public void accepts_any_positive()
        {
            new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", "text/html, */*").AcceptsAny().ShouldBeTrue();
        }

        [Test]
        public void accepts_any_if_no_accept_header_specified()
        {
            new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", null).AcceptsAny().ShouldBeTrue();
        }

        [Test]
        public void accepts_any_if_accept_header_is_all_spaces()
        {
            new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", "  ").AcceptsAny().ShouldBeTrue();
        }

        [Test]
        public void accepts_any_if_accept_header_is_empty()
        {
            new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", string.Empty).AcceptsAny().ShouldBeTrue();
        }

        [Test]
        public void select_first_matching_no_matches()
        {
            new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", "text/html")
                .SelectFirstMatching(new[]{"application/json"}).ShouldBeNull();
        }

        [Test]
        public void select_first_with_a_wild_card()
        {
            var currentMimeType = new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", "text/html, */*");
            currentMimeType
                .SelectFirstMatching(new[]{"text/json", "application/json"})
                .ShouldBe("text/json");

            currentMimeType
                .SelectFirstMatching(new[] { "application/json", "text/json" })
                .ShouldBe("application/json");
        
        }

        [Test]
        public void select_first_when_one_does_match()
        {
            var currentMimeType = new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", "text/html, application/json, */*");
            currentMimeType
                .SelectFirstMatching(new[] { "text/json", "application/json" })
                .ShouldBe("application/json"); 
        }
    }
}