using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Syndication;
using FubuCore;
using FubuLocalization;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;
using FubuMVC.Media.Xml;

namespace FubuMVC.Media.Atom
{
    public class Feed<T> : IResourceRegistration, IFeedDefinition<T>
    {
        private readonly IList<Action<SyndicationFeed>> _alterations = new List<Action<SyndicationFeed>>();
        private readonly Lazy<XmlProjection<T>> _extension = new Lazy<XmlProjection<T>>(() => new XmlProjection<T>());
        private readonly IList<ILinkCreator> _links = new List<ILinkCreator>();
        private IFeedItem<T> _itemConfiguration = new FeedItem<T>();

        public Feed()
        {
            ContentType = "application/atom+xml";
        }

        // TODO -- do something with the Updated date


        private Action<SyndicationFeed> alter
        {
            set { _alterations.Add(value); }
        }

        public FeedItem<T> Items
        {
            get { return (FeedItem<T>) _itemConfiguration; }
        }


        public XmlProjection<T> Extension
        {
            get { return _extension.Value; }
        }

        /// <summary>
        /// Matches the accepted mimetypes in the client request.  Can be a comma delimited
        /// value for multiple mimetypes.  Default is "application/atom+xml"
        /// </summary>
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
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(T));
            graph.OutputNodesFor(enumerableType).Each(outputNode =>
            {
                outputNode.Writers.AddToEnd(new FeedWriterNode<T>(this, outputNode.ResourceType));
            });
        }

        public LinkExpression Link(object target)
        {
            return Link(urls => urls.UrlFor(target));
        }

        public LinkExpression Link<TController>(Expression<Action<TController>> method)
        {
            return Link(urls => urls.UrlFor(method, null));
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
    }
}