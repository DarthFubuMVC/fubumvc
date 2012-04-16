using System.ServiceModel.Syndication;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Urls;

namespace FubuMVC.Media.Atom
{
    public interface IFeedDefinition<T>
    {
        string ContentType { get; }
        void ConfigureFeed(SyndicationFeed feed, IUrlRegistry urls);
        void ConfigureItem(SyndicationItem item, IValues<T> values);
    }
}