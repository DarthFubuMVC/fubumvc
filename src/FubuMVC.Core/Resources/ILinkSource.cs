using System.Collections.Generic;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Resources
{
    public interface ILinkSource<T>
    {
        IEnumerable<Link> LinksFor(IValues<T> target, IUrlRegistry urls);
    }
}