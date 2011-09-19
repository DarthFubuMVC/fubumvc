using System.Collections.Generic;
using System.ServiceModel.Syndication;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest
{
    public interface ILinkSource<T>
    {
        IEnumerable<SyndicationLink> LinksFor(IValues<T> target, IUrlRegistry urls);
    }
}