using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using FubuLocalization;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Rest.Media.Xml;
using FubuCore;

namespace FubuMVC.Core.Rest.Media.Atom
{
    public interface IFeedDefinition<T>
    {
        IXmlMediaWriter<T> BuildExtensionWriter();

        // TODO -- think we need to inject a system date time provider thingie
        void ConfigureFeed(SyndicationFeed feed);
        void ConfigureItem(SyndicationItem item, IValues<T> values);
    }


    public class Feed<T> : IResourceRegistration, IFeedDefinition<T>
    {
        private readonly IList<Action<SyndicationFeed>> _alterations = new List<Action<SyndicationFeed>>();
        private XmlProjection<T> _extension;
        private IFeedItem<T> _itemConfiguration;

        private Action<SyndicationFeed> alter
        {
            set { _alterations.Add(value); }
        }

        IXmlMediaWriter<T> IFeedDefinition<T>.BuildExtensionWriter()
        {
            throw new NotImplementedException();
        }

        void IFeedDefinition<T>.ConfigureFeed(SyndicationFeed feed)
        {
            // TODO -- put something in FubuCore for this.  Too common not to
            _alterations.Each(x => x(feed));
        }

        void IFeedDefinition<T>.ConfigureItem(SyndicationItem item, IValues<T> values)
        {
            _itemConfiguration.ConfigureItem(item, values);
        }

        void IResourceRegistration.Modify(ConnegGraph graph, BehaviorGraph behaviorGraph)
        {
            throw new NotImplementedException();
        }

        public void Title(StringToken title)
        {
            alter = feed => feed.Title = title.ToString().ToContent();
        }

        public void Description(StringToken description)
        {
            alter = feed => feed.Description = description.ToString().ToContent();
        }

        public void Items<TMap>() where TMap : FeedItem<T>, new()
        {
            _itemConfiguration = new TMap();
        }

        public void Items(Action<FeedItem<T>> configure)
        {
            var itemMap = new FeedItem<T>();
            configure(itemMap);

            _itemConfiguration = itemMap;
        }

        public void Extension(Action<XmlProjection<T>> configure)
        {
            _extension = new XmlProjection<T>();
            configure(_extension);
        }
    }

    public static class SyndicationExtensions
    {
        public static TextSyndicationContent ToContent(this string text)
        {
            return new TextSyndicationContent(text);
        }

        public static SyndicationLink ToSyndicationLink(this Link link)
        {
            var syndicationLink = new SyndicationLink(new Uri(link.Url));
            link.Rel.IfNotNull(x => syndicationLink.RelationshipType = x);
            link.Title.IfNotNull(x => syndicationLink.Title = x);
            link.Mimetype.IfNotNull(x => syndicationLink.MediaType = x);

            return syndicationLink;
        }
    }
}