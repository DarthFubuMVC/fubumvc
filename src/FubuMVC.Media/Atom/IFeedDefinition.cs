using System.ServiceModel.Syndication;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Atom
{
    public interface IFeedDefinition<T>
    {
        string ContentType { get; }
        void ConfigureFeed(SyndicationFeed feed, IUrlRegistry urls);
        void ConfigureItem(SyndicationItem item, IValues<T> values);
    }
}