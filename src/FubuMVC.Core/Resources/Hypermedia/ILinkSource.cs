using System.Collections.Generic;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Resources.Hypermedia
{
    public interface ILinkSource<T>
    {
        IEnumerable<Link> LinksFor(IValues<T> target, IUrlRegistry urls);
    }
}