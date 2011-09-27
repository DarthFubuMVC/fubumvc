using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using FubuLocalization;
using FubuMVC.Core.Rest;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Atom;
using FubuMVC.Core.Rest.Media.Xml;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Rest.Media.Atom
{
    [TestFixture]
    public class when_writing_a_feed_without_any_extensions
    {
        private ValidStubUrlRegistry theUrls;
        private SyndicationFeed theResultingFeed;

        [TestFixtureSetUp]
        public void SetUp()
        {
            theUrls = new ValidStubUrlRegistry();
            var writer = new FeedWriter<FeedTarget>(
                new FeedTargetSource(),
                new FeedWithoutExtension(),
                new FeedTargetLinks(),
                theUrls);

            theResultingFeed = writer.BuildFeed();
        }

        [Test]
        public void the_feed_has_the_right_number_of_items()
        {
            theResultingFeed.Items.Count().ShouldEqual(3);
        }

        [Test]
        public void wrote_the_feed_level_data()
        {
            theResultingFeed.Title.Text.ShouldEqual("Some title");
            
        }

        [Test]
        public void applied_feed_level_links()
        {
            theResultingFeed.Links.Select(x => x.Uri.OriginalString)
                .ShouldHaveTheSameElementsAs(theUrls.UrlFor(new FeedRoot()), theUrls.UrlFor<FeedRoot>(x => x.Go()));

            theResultingFeed.Links.First().RelationshipType.ShouldEqual("root");
        }

        [Test]
        public void applied_item_level_links()
        {
            theResultingFeed.Items.Each(x => {
                 x.Links.Count.ShouldEqual(1);
            });

            theResultingFeed.Items.Select(x => x.Links.Single().Uri.OriginalString)
                .ShouldHaveTheSameElementsAs("http://somewhere.com/City: Austin","http://somewhere.com/City: Dallas","http://somewhere.com/City: Houston");
        }

        [Test]
        public void there_should_be_no_extensions_on_any_item()
        {
            theResultingFeed.Items.Each(x => x.ElementExtensions.Any().ShouldBeFalse());
        }
    }

    [TestFixture]
    public class when_writing_a_feed_with_extensions
    {
        private ValidStubUrlRegistry theUrls;
        private SyndicationFeed theResultingFeed;

        [TestFixtureSetUp]
        public void SetUp()
        {
            theUrls = new ValidStubUrlRegistry();
            var writer = new FeedWriter<FeedTarget>(
                new FeedTargetSource(),
                new FeedWithExtension(),
                new FeedTargetLinks(),
                theUrls);

            theResultingFeed = writer.BuildFeed();
        }

        [Test]
        public void the_resulting_feed_should_have_the_extensions()
        {
            var extension = theResultingFeed.Items.ElementAt(0).ElementExtensions.Single().ShouldBeOfType<SyndicationElementExtension>();
            extension.ShouldNotBeNull();

            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder);
            var formatter = new Atom10FeedFormatter(theResultingFeed);

            formatter.WriteTo(writer);
            writer.Close();

            builder.ToString().ShouldContain("<Item xmlns=\"\"><City>Dallas</City><Name>The second item</Name></Item>");
        }
    }

    public class FeedWithoutExtension : Feed<FeedTarget>
    {
        public FeedWithoutExtension()
        {
            Title(StringToken.FromKeyString("TITLE", "Some title"));

            Link(new FeedRoot()).Rel("root");
            Link<FeedRoot>(x => x.Go());

            Items.Id(o => o.Number);
            Items.Title(o => o.Title);
        }
    }

    public class FeedWithExtension : FeedWithoutExtension
    {
        public FeedWithExtension()
        {
            Extension.Root = "Item";
            Extension.NodeStyle = XmlNodeStyle.NodeCentric;
            Extension.Value(x => x.City);
            Extension.Value(x => x.Name);
        }
    }


    public class FeedTargetSource : IFeedSource<FeedTarget>
    {
        public IEnumerable<IValues<FeedTarget>> GetValues()
        {
            yield return new FeedTarget{
                City = "Austin",
                Title = "Item 1",
                Name = "The first item",
                Number = 1
            }.ToValues();

            yield return new FeedTarget
            {
                City = "Dallas",
                Title = "Item 2",
                Name = "The second item",
                Number = 2
            }.ToValues();

            yield return new FeedTarget
            {
                City = "Houston",
                Title = "Item 3",
                Name = "The third item",
                Number = 3
            }.ToValues();
        }
    }

    public class FeedTargetLinks : LinksSource<FeedTarget>
    {
        public FeedTargetLinks()
        {
            ToSubject().Rel("self");
        }
    }

    public class FeedRoot
    {
        public void Go(){}    
    }

    public class FeedTarget
    {
        public string Title { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }

        public override string ToString()
        {
            return string.Format("City: {0}", City);
        }
    }
}