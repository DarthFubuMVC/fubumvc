using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Resources.Hypermedia
{
    public interface ILinkCreator
    {
        Link CreateLink(IUrlRegistry urls);
    }
}