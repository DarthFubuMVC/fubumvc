using System.Collections.Generic;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests.RazorModel.Policies
{
    [TestFixture]
    public class ViewPathPolicyTester : InteractionContext<ViewPathPolicy<IRazorTemplate>>
    {
        [Test]
        public void when_origin_is_host_prefix_is_emtpy()
        {
            var item = new Template("", "", TemplateConstants.HostOrigin);
            ClassUnderTest.Apply(item);
            item.ViewPath.ShouldBeEmpty();
        }

        [Test]
        public void when_origin_is_not_host_prefix_is_not_emtpy()
        {
            var item = new Template("", "", "Foo");
            ClassUnderTest.Apply(item);
            item.ViewPath.ShouldNotBeEmpty();
        }

        [Test]
        public void the_items_of_the_same_origin_have_the_same_prefix()
        {
            var baz1 = new Template("", "", "Baz");
            var baz2 = new Template("", "", "Baz");
            var bar1 = new Template("", "", "Bar");
            var bar2 = new Template("", "", "Bar");

            new[] { baz1, baz2, bar1, bar2 }.Each(x => ClassUnderTest.Apply(x));

            baz1.ViewPath.ShouldEqual(baz2.ViewPath);
            bar1.ViewPath.ShouldEqual(bar2.ViewPath);
            baz1.ViewPath.ShouldNotEqual(bar1.ViewPath);
        }
		
		[Test]
        public void it_matches_when_viewpath_is_empty()
        {
            var item = new Template("", "", "Foo");
			ClassUnderTest.Matches(item).ShouldBeTrue();
            ClassUnderTest.Apply(item);
			ClassUnderTest.Matches(item).ShouldBeFalse();
        }
    }
}