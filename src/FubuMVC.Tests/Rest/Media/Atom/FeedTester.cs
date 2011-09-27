using System;
using System.ServiceModel.Syndication;
using FubuCore;
using FubuLocalization;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Atom;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Rest.Media.Atom
{
    [TestFixture]
    public class FeedTester
    {
        private Feed<ItemSubject> theFeed;
        private IFeedDefinition<ItemSubject> theDefinition;
        private SyndicationFeed theSyndicationFeed;
        private SyndicationItem aSyndicationItem;
        private ValidStubUrlRegistry theUrls;

        [SetUp]
        public void SetUp()
        {
            theFeed = new Feed<ItemSubject>();
            theDefinition = theFeed.As<IFeedDefinition<ItemSubject>>();

            theSyndicationFeed = new SyndicationFeed();

            aSyndicationItem = new SyndicationItem();

            theUrls = new ValidStubUrlRegistry();
        }

        [Test]
        public void title()
        {
            var token = StringToken.FromKeyString("KEY1", "The default value");
            theFeed.Title(token);

            theDefinition.ConfigureFeed(theSyndicationFeed, theUrls);

            theSyndicationFeed.Title.Text.ShouldEqual(token.ToString());
        }

        [Test]
        public void description()
        {
            var token = StringToken.FromKeyString("KEY1", "The default value");
            theFeed.Description(token);

            theDefinition.ConfigureFeed(theSyndicationFeed, theUrls);

            theSyndicationFeed.Description.Text.ShouldEqual(token.ToString());
        }

        [Test]
        public void configure_item_when_using_an_externally_defined_feed_item()
        {
            theFeed.Items<ItemSubjectMap>();
            var values = new SimpleValues<ItemSubject>(new ItemSubject(){
                Title = "Something"
            });

            theDefinition.ConfigureItem(aSyndicationItem, values);

            aSyndicationItem.Title.Text.ShouldEqual("Something");
        }

        [Test]
        public void configure_items_when_the_item_map_is_definied_inline()
        {
            theFeed.Items(x =>
            {
                x.Title(o => o.Title);
            });

            var values = new SimpleValues<ItemSubject>(new ItemSubject()
            {
                Title = "Else"
            });

            theDefinition.ConfigureItem(aSyndicationItem, values);

            aSyndicationItem.Title.Text.ShouldEqual("Else");
        }

        [Test]
        public void write_links()
        {
            // Nothing up our sleeve
            theDefinition.ConfigureFeed(theSyndicationFeed, new StubUrlRegistry());
            theSyndicationFeed.Links.Any().ShouldBeFalse();

            theFeed.Link(new UrlTarget()).Rel("self");
            theFeed.Link<UrlTarget>(x => x.Go()).Rel("go");

            theDefinition.ConfigureFeed(theSyndicationFeed, new ValidStubUrlRegistry());
            theSyndicationFeed.Links.Select(x => x.RelationshipType)
                .ShouldHaveTheSameElementsAs("self", "go");
        }
    }

    public class UrlTarget
    {
        public override string ToString()
        {
            return "UrlTarget";
        }

        public void Go()
        {
            
        }
    }

    public class ItemSubjectMap : FeedItem<ItemSubject>
    {
        public ItemSubjectMap()
        {
            Title(x => x.Title);
        }
    }
}