using System.Collections.Generic;
using System.Xml;
using FubuCore;
using FubuMVC.Core.Rest.Media.Projections;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest.Media.Xml
{
    public interface IXmlMediaWriter<T>
    {
        XmlDocument Write(IValues<T> values);
    }

    public class XmlMediaWriter<T> : MediaWriter<T>, IXmlMediaWriter<T>
    {
        public XmlMediaWriter(XmlMediaOptions options, ILinkSource<T> links, IUrlRegistry urls,
                              IValueProjection<T> projection)
            : base(new XmlMediaDocument(options), links, urls, projection)
        {
        }

        public XmlDocument Write(IValues<T> values)
        {
            writeData(values);

            return document.As<XmlMediaDocument>().Document;
        }
    }


    // Needs to be created per request!!!!
    public class XmlMediaDocument : IMediaDocument
    {
        private readonly XmlDocument _document;
        private readonly XmlMediaOptions _options;
        private readonly IXmlMediaNode _topNode;

        public XmlMediaDocument(XmlMediaOptions options)
        {
            _document = new XmlDocument();
            if (options.Namespace.IsEmpty())
            {
                _document.WithRoot(options.Root);
            }
            else
            {
                var node = _document.CreateNode(XmlNodeType.Element, options.Root, options.Namespace);
                _document.AppendChild(node);
            }


            _topNode = options.NodeStyle == XmlNodeStyle.AttributeCentric
                           ? (IXmlMediaNode) new XmlAttCentricMediaNode(_document.DocumentElement)
                           : new XmlNodeCentricMediaNode(_document.DocumentElement);

            _topNode.LinkWriter = options.LinkWriter;

            _options = options;
        }


        public XmlDocument Document
        {
            get { return _document; }
        }

        public IMediaNode Root
        {
            get { return _topNode; }
        }

        public void Write(IOutputWriter writer)
        {
            writer.Write(_options.Mimetype, _document.OuterXml);
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return _options.Mimetype; }
        }
    }
}