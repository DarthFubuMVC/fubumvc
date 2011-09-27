using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using FubuCore;
using FubuLocalization;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Atom;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using FubuMVC.Core.Rest.Conneg;

namespace FubuMVC.Tests.Rest.Media.Atom
{
    [TestFixture]
    public class when_modifying_resource_nodes_matching_a_feed_target
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();
            registry.Media.ApplyContentNegotiationToActions(call => true);

            theGraph = registry.BuildGraph();
        }

        [Test]
        public void should_apply_a_media_output_node_to_enumerables_of_the_target_type()
        {
            var outputNode = theGraph.BehaviorFor<Controller1>(x => x.M1())
                .ConnegOutputNode()
                .Writers.Single()
                .ShouldBeOfType<FeedWriterNode<TargetClass>>();

            outputNode.FeedSourceType.ShouldEqual(typeof (EnumerableFeedSource<EnumerableOutput, TargetClass>));
            outputNode.Feed.ShouldBeOfType<TargetClassFeed>();
        }

        [Test]
        public void should_apply_a_media_output_node_to_enumerables_of_values_of_the_target_type()
        {
            var outputNode = theGraph.BehaviorFor<Controller1>(x => x.M2())
                .ConnegOutputNode()
                .Writers.Single()
                .ShouldBeOfType<FeedWriterNode<TargetClass>>();

            outputNode.FeedSourceType.ShouldEqual(typeof(DirectFeedSource<EnumerableValuesOutput, TargetClass>));
            outputNode.Feed.ShouldBeOfType<TargetClassFeed>();
        }

        public class TargetClassFeed : Feed<TargetClass>
        {
            public TargetClassFeed()
            {
                Debug.WriteLine("I was built");
            }
        }
    
        public class EnumerableOutput : IEnumerable<TargetClass>
        {
            IEnumerator<TargetClass> IEnumerable<TargetClass>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public class EnumerableValuesOutput : IEnumerable<IValues<TargetClass>>
        {
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<IValues<TargetClass>> GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public class TargetClass{}

        public class Controller1
        {
            public EnumerableOutput M1()
            {
                return new EnumerableOutput();
            }

            public EnumerableValuesOutput M2()
            {
                return null;
            }

        }
    }

    


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
        public void content_type_is_atom_pub_by_default()
        {
            theFeed.ContentType.ShouldEqual("application/atom+xml");
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
            theFeed.UseItems<ItemSubjectMap>();
            var values = new SimpleValues<ItemSubject>(new ItemSubject(){
                Title = "Something"
            });

            theDefinition.ConfigureItem(aSyndicationItem, values);

            aSyndicationItem.Title.Text.ShouldEqual("Something");
        }

        [Test]
        public void configure_items_when_the_item_map_is_definied_inline()
        {
            theFeed.Items.Title(o => o.Title);

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