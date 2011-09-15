using System.Collections.Generic;
using System.ServiceModel.Syndication;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest
{
    public interface ILinkSource<T>
    {
        IEnumerable<SyndicationLink> LinksFor(IValueSource<T> target, IUrlRegistry urls);
    }
}