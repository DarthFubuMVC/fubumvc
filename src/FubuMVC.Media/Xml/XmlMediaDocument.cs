using System.Collections.Generic;
using System.Xml;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Xml
{
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

        public void Write(IOutputWriter writer, string mimeType)
        {
            writer.Write(_options.Mimetype, _document.OuterXml);
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _options.Mimetype.ToDelimitedArray(','); }
        }
    }
}