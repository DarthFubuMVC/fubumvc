using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using FubuCore;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Atom
{
    public class FeedWriter<T> : IMediaWriter<IEnumerable<T>>
    {
        private readonly IFeedDefinition<T> _definition;
        private readonly ILinkSource<T> _links;
        private readonly IUrlRegistry _urls;
        private readonly IOutputWriter _writer;

        public FeedWriter(IFeedDefinition<T> definition, ILinkSource<T> links, IUrlRegistry urls, IOutputWriter writer)
        {
            _definition = definition;
            _links = links;
            _urls = urls;
            _writer = writer;
        }

        public void Write(string mimeType, IEnumerable<T> resource)
        {
            var source = new EnumerableFeedSource<T>(resource);
            var feed = BuildFeed(source);

            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder);
            var formatter = new Atom10FeedFormatter(feed);

            formatter.WriteTo(writer);
            writer.Close();

            _writer.Write(mimeType, builder.ToString());
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _definition.ContentType.ToDelimitedArray(','); }
        }

        public virtual SyndicationFeed BuildFeed(IFeedSource<T> source)
        {
            var feed = new SyndicationFeed();

            _definition.ConfigureFeed(feed, _urls);
            feed.Items = buildItems(source);

            return feed;
        }

        private IEnumerable<SyndicationItem> buildItems(IFeedSource<T> source)
        {
            return source.GetValues().Select(buildItem);
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
    }
}