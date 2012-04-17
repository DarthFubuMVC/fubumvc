using System.Collections.Generic;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media
{
    public interface ILinkSource<T>
    {
        IEnumerable<Link> LinksFor(IValues<T> target, IUrlRegistry urls);
    }
}