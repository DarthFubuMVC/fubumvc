using System;
using System.ServiceModel.Syndication;
using FubuCore;
using FubuLocalization;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Atom;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Rest.Media.Atom
{
    [TestFixture]
    public class FeedTester
    {
        private Feed<ItemSubject> theFeed;
        private IFeedDefinition<ItemSubject> theDefinition;
        private SyndicationFeed theSyndicationFeed;
        private SyndicationItem aSyndicationItem;

        [SetUp]
        public void SetUp()
        {
            theFeed = new Feed<ItemSubject>();
            theDefinition = theFeed.As<IFeedDefinition<ItemSubject>>();

            theSyndicationFeed = new SyndicationFeed();

            aSyndicationItem = new SyndicationItem();
        }

        [Test]
        public void title()
        {
            var token = StringToken.FromKeyString("KEY1", "The default value");
            theFeed.Title(token);

            theDefinition.ConfigureFeed(theSyndicationFeed);

            theSyndicationFeed.Title.Text.ShouldEqual(token.ToString());
        }

        [Test]
        public void description()
        {
            var token = StringToken.FromKeyString("KEY1", "The default value");
            theFeed.Description(token);

            theDefinition.ConfigureFeed(theSyndicationFeed);

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
    }

    public class ItemSubjectMap : FeedItem<ItemSubject>
    {
        public ItemSubjectMap()
        {
            Title(x => x.Title);
        }
    }
}