using System.Collections.Generic;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest
{
    public interface ILinkSource<T>
    {
        IEnumerable<Link> LinksFor(IValues<T> target, IUrlRegistry urls);
    }
}