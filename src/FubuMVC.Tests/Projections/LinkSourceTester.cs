using System;
using System.Linq;
using FubuCore;
using FubuLocalization;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class LinkSourceTester
    {
        private Site theSubject;
        private SimpleProjectionTarget theTarget;

        [SetUp]
        public void SetUp()
        {
            theSubject = new Site();
            theTarget = new SimpleProjectionTarget(theSubject, new StubUrlRegistry());
        }

        [Test]
        public void create_a_link_if_the_filter_passes()
        {
            var link = new LinkSource<Site>(t => "http://site.com")
                .IfSubjectMatches(s => true);

            link.As<ILinkSource>().LinksFor(theTarget)
                .Single()
                .ShouldNotBeNull();
        }

        [Test]
        public void do_not_create_a_link_if_the_filter_fails()
        {
            var link = new LinkSource<Site>(t => "http://site.com")
                .IfSubjectMatches(s => false);

            link.As<ILinkSource>().LinksFor(theTarget)
                .Any().ShouldBeFalse();
        }

        [Test]
        public void do_not_create_a_link_if_the_filter_fails_by_target_matches()
        {
            theSubject.IsObsolete = true;

            var link = new LinkSource<Site>(t => "http://site.com")
                .IfSubjectMatches(s => !s.IsObsolete);

            link.As<ILinkSource>().LinksFor(theTarget)
                .Any().ShouldBeFalse();
        }

        [Test]
        public void do_not_create_a_link_if_the_filter_fails_by_IfEquals()
        {
            theSubject.Name = "Jeremy";

            var link = new LinkSource<Site>(t => "http://site.com")
                .IfEquals(x => x.Name, "Chad");

            link.As<ILinkSource>().LinksFor(theTarget)
                .Any().ShouldBeFalse();
        }


        [Test]
        public void do_create_a_link_if_the_filter_succeeds_by_IfEquals()
        {
            theSubject.Name = "Jeremy";

            var link = new LinkSource<Site>(t => "http://site.com")
                .IfEquals(x => x.Name, "Jeremy");

            link.As<ILinkSource>().LinksFor(theTarget)
                .Any().ShouldBeTrue();
        }

        [Test]
        public void do_not_create_a_link_if_the_filter_fails_by_IfNotEquals()
        {
            theSubject.Name = "Jeremy";

            var link = new LinkSource<Site>(t => "http://site.com")
                .IfNotEquals(x => x.Name, "Chad");

            link.As<ILinkSource>().LinksFor(theTarget)
                .Any().ShouldBeTrue();
        }


        [Test]
        public void do_create_a_link_if_the_filter_succeeds_by_IfNotEquals()
        {
            theSubject.Name = "Jeremy";

            var link = new LinkSource<Site>(t => "http://site.com")
                .IfNotEquals(x => x.Name, "Jeremy");

            link.As<ILinkSource>().LinksFor(theTarget)
                .Any().ShouldBeFalse();
        }

        [Test]
        public void create_a_link_with_just_the_url_from_the_target()
        {
            var source = new LinkSource<Site>(t => "http://site.com");

            var link = source.As<ILinkSource>().LinksFor(theTarget)
                .Single();

            link.Uri.ShouldEqual(new Uri("http://site.com"));
            link.RelationshipType.ShouldBeNull();
            link.Title.ShouldBeNull();
        }

        [Test]
        public void create_a_link_with_just_the_url_from_the_target_with_a_relationship_type()
        {
            var source = new LinkSource<Site>(t => "http://site.com")
                .Rel("something");

            var link = source.As<ILinkSource>().LinksFor(theTarget)
                .Single();

            link.Uri.ShouldEqual(new Uri("http://site.com"));
            link.RelationshipType.ShouldEqual("something");
            link.Title.ShouldBeNull();
        }


        [Test]
        public void create_a_link_with_just_the_url_from_the_target_with_a_title()
        {
            var token = StringToken.FromKeyString("the title");

            var source = new LinkSource<Site>(t => "http://site.com")
                .Rel("something")
                .Title(token);

            var link = source.As<ILinkSource>().LinksFor(theTarget)
                .Single();

            link.Uri.ShouldEqual(new Uri("http://site.com"));
            link.Title.ShouldEqual(token.ToString());
        }


    }

    public class Site
    {
        public Guid Id { get; set; }
        public bool IsObsolete { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("Site: {0}", Name);
        }
    }
}