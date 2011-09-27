using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest.Media.Atom
{
    public class FeedWriter<T> : IMediaWriter<T>
    {
        private readonly IFeedDefinition<T> _definition;
        private readonly IFeedSource<T> _feedSource;
        private readonly ILinkSource<T> _links;
        private readonly IUrlRegistry _urls;

        public FeedWriter(IFeedSource<T> feedSource, IFeedDefinition<T> definition, ILinkSource<T> links,
                          IUrlRegistry urls)
        {
            _feedSource = feedSource;
            _definition = definition;
            _links = links;
            _urls = urls;
        }


        public IEnumerable<string> Mimetypes
        {
            get { yield return _definition.ContentType; }
        }

        public void Write(IValues<T> source, IOutputWriter writer)
        {
            Write(writer);
        }

        public void Write(T source, IOutputWriter writer)
        {
            Write(writer);
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
    }
}