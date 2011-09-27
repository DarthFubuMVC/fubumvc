using System;
using System.Collections.Generic;
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

        public void Write(IOutputWriter writer)
        {
            

            var feed = new SyndicationFeed();
            
            // TODO -- this should put the links on too
            _definition.ConfigureFeed(feed, _urls);

            Action<SyndicationItem, IValues<T>> extendItem = (item, values) => { };
            var extensionWriter = _definition.BuildExtensionWriter();
            if (extensionWriter != null)
            {
                extendItem = (item, values) =>
                {
                    var doc = extensionWriter.WriteValues(values);
                    item.ElementExtensions.Add(doc);
                };
            }



            _feedSource.GetValues().Each(values =>
            {
                var item = new SyndicationItem();
                _definition.ConfigureItem(item, values);

                var links = _links.LinksFor(values, _urls).Select(x => x.ToSyndicationLink());
                item.Links.AddRange(links);

                extendItem(item, values);
            });
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
        IXmlMediaWriter<T> BuildExtensionWriter();

        // TODO -- think we need to inject a system date time provider thingie
        void ConfigureFeed(SyndicationFeed feed, IUrlRegistry urls);
        void ConfigureItem(SyndicationItem item, IValues<T> values);
        string ContentType { get; }
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

        // TODO -- test that this monster is set by default to atom pub
        public string ContentType { get; set; }

        IXmlMediaWriter<T> IFeedDefinition<T>.BuildExtensionWriter()
        {
            throw new NotImplementedException();
        }

        void IFeedDefinition<T>.ConfigureFeed(SyndicationFeed feed, IUrlRegistry urls)
        {
            // TODO -- put links in here too.

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
            link.ContentType.IfNotNull(x => syndicationLink.MediaType = x);

            return syndicationLink;
        }
    }
}