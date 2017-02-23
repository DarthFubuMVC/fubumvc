using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Hypermedia;
using FubuMVC.Core.Urls;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Resources.Hypermedia
{
    
    public class LinksSourceTester
    {
        private Site theSubject;
        private SimpleValues<Site> theTarget;
        private ValidStubUrlRegistry theUrls;
        private LinksSource<Site> theLinks;

        public LinksSourceTester()
        {
            theSubject = new Site(){Name = "my site", Id = Guid.NewGuid()};
            theUrls = new ValidStubUrlRegistry();
            theTarget = new SimpleValues<Site>(theSubject);

            theLinks = new LinksSource<Site>();
        }

        [Fact]
        public void create_link_by_subject()
        {
            theLinks.ToSubject();

            theLinks.As<ILinkSource<Site>>().LinksFor(theTarget, theUrls)
                .Single().Url.ShouldBe(theUrls.UrlFor(theSubject));
        }

        [Fact]
        public void create_a_link_by_using_route_parameters()
        {
            theLinks.ToInput<SiteAction>(x => x.Name);

            var parameters = new RouteParameters<SiteAction>();
            parameters["Name"] = theSubject.Name;

            theLinks.As<ILinkSource<Site>>().LinksFor(theTarget, theUrls)
                .Single().Url.ShouldBe(theUrls.UrlFor(parameters));
        }

        [Fact]
        public void create_link_by_transforming_the_subject()
        {
            theLinks.To(site => new SiteAction(site.Name));

            theLinks.As<ILinkSource<Site>>().LinksFor(theTarget, theUrls)
                .Single().Url.ShouldBe(theUrls.UrlFor(new SiteAction(theSubject.Name)));
        }

        [Fact]
        public void create_link_by_identifier()
        {
            theLinks.ToSubject(x => x.Id);

            var parameters = new RouteParameters<Site>();
            parameters[x => x.Id] = theSubject.Id.ToString();

            theLinks.As<ILinkSource<Site>>().LinksFor(theTarget, theUrls)
                .Single().Url.ShouldBe(theUrls.UrlFor(typeof(Site), parameters));
        }

        public class SiteAction
        {
            private readonly string _name;

            public SiteAction(string name)
            {
                _name = name;
            }

            public override string ToString()
            {
                return string.Format("site action for {0}", _name);
            }
        }
    }
}