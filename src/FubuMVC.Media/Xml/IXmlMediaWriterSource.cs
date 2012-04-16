using FubuMVC.Core.Urls;

namespace FubuMVC.Media.Xml
{
    public interface IXmlMediaWriterSource<T>
    {
        IXmlMediaWriter<T> BuildWriter();
        IXmlMediaWriter<T> BuildWriterFor(ILinkSource<T> links, IUrlRegistry urls);
    }
}