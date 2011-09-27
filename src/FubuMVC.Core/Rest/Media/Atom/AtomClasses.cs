using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ServiceModel.Syndication;
using FubuLocalization;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Rest.Media.Xml;
using FubuCore;
using FubuMVC.Core.Runtime;
using System.Linq;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest.Media.Atom
{
    public class FeedWriter<T> : IMediaWriter<T>
    {
        private readonly IFeedSource<T> _feedSource;
        private readonly IFeedDefinition<T> _definition;
        private readonly ILinkSource<T> _links;
        private readonly IUrlRegistry _urls;

        public FeedWriter(IFeedSource<T> feedSource, IFeedDefinition<T> definition, ILinkSource<T> links, IUrlRegistry urls)
        {
            _feedSource = feedSource;
            _definition = definition;
            _links = links;
            _urls = urls;
        }


        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return _definition.ContentType;
            }
        }

        public virtual SyndicationFeed BuildFeed()
        {
            var feed = new SyndicationFeed();

            _definition.ConfigureFeed(feed, _urls);
            feed.Items = buildItems();

            return feed;
        }

        private IEnumerable<SyndicationItem> buildItems()
        {
            foreach (var values in _feedSource.GetValues())
            {
                yield return buildItem(values);
            }
        }

        private SyndicationItem buildItem(IValues<T> values)
        {
            var item = new SyndicationItem();
            _definition.ConfigureItem(item, values);

            addLinksForItem(item, values);

            return item;
        }

        private void addLinksForItem(SyndicationItem item, IValues<T> values)
        {
            var links = _links.LinksFor(values, _urls).Select(x => x.ToSyndicationLink());
            item.Links.AddRange(links);
        }

        public void Write(IOutputWriter writer)
        {
            
            throw new NotImplementedException();

        }

        public void Write(IValues<T> source, IOutputWriter writer)
        {
            Write(writer);
        }

        public void Write(T source, IOutputWriter writer)
        {
            Write(writer);
        }
    }



    public interface IFeedDefinition<T>
    {
        void ConfigureFeed(SyndicationFeed feed, IUrlRegistry urls);
        void ConfigureItem(SyndicationItem item, IValues<T> values);
        string ContentType { get; }
    }


    public class Feed<T> : IResourceRegistration, IFeedDefinition<T>
    {
        private readonly IList<Action<SyndicationFeed>> _alterations = new List<Action<SyndicationFeed>>();
        private readonly Lazy<XmlProjection<T>> _extension = new Lazy<XmlProjection<T>>(() => new XmlProjection<T>());
        private IFeedItem<T> _itemConfiguration = new FeedItem<T>();
        private readonly IList<ILinkCreator> _links = new List<ILinkCreator>();


        // TODO -- do something with the Updated date


        private Action<SyndicationFeed> alter
        {
            set { _alterations.Add(value); }
        }

        // TODO -- test that this monster is set by default to atom pub
        public string ContentType { get; set; }

        void IFeedDefinition<T>.ConfigureFeed(SyndicationFeed feed, IUrlRegistry urls)
        {
            feed.Links.Clear();
            var syndicationLinks = _links.Select(x => x.CreateLink(urls).ToSyndicationLink());
            feed.Links.AddRange(syndicationLinks);

            // TODO -- put something in FubuCore for this.  Too common not to
            _alterations.Each(x => x(feed));
        }

        void IFeedDefinition<T>.ConfigureItem(SyndicationItem item, IValues<T> values)
        {
            _itemConfiguration.ConfigureItem(item, values);


            if (_extension.IsValueCreated)
            {
                var writer = _extension.Value.As<IXmlMediaWriterSource<T>>().BuildWriter();
                var doc = writer.WriteValues(values);

                item.ElementExtensions.Add(doc.DocumentElement);
            }
        }

        void IResourceRegistration.Modify(ConnegGraph graph, BehaviorGraph behaviorGraph)
        {
            
        }

        public LinkExpression Link(object target)
        {
            return Link(urls => urls.UrlFor(target));
        }

        public LinkExpression Link<TController>(Expression<Action<TController>> method)
        {
            return Link(urls => urls.UrlFor(method));
        }

        public LinkExpression Link(Func<IUrlRegistry, string> urlsource)
        {
            var expression = new LinkExpression(urlsource);
            _links.Add(expression);

            return expression;
        }

        public void Title(StringToken title)
        {
            alter = feed => feed.Title = title.ToString().ToContent();
        }

        public void Description(StringToken description)
        {
            alter = feed => feed.Description = description.ToString().ToContent();
        }

        public void UseItems<TMap>() where TMap : FeedItem<T>, new()
        {
            _itemConfiguration = new TMap();
        }

        public FeedItem<T> Items
        {
            get { return (FeedItem<T>) _itemConfiguration; }
        }


        public XmlProjection<T> Extension
        {
            get { return _extension.Value; }
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
            link.ContentType.IfNotNull(x => syndicationLink.MediaType = x);

            return syndicationLink;
        }
    }
}