using System.Collections.Generic;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Urls;

namespace FubuMVC.Media
{
    public interface ILinkSource<T>
    {
        IEnumerable<Link> LinksFor(IValues<T> target, IUrlRegistry urls);
    }
}