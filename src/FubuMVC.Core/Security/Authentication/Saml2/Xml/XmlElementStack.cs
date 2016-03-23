using System;
using System.Collections.Generic;
using System.Xml;
using FubuCore;

namespace FubuMVC.Core.Security.Authentication.Saml2.Xml
{
    public class XmlElementStack : ReadsSamlXml
    {
        private readonly string _xsd;
        private readonly Stack<XmlElement> _elements = new Stack<XmlElement>();
        private readonly XmlDocument _document;
        private string _prefix;

        public XmlElementStack(XmlDocument document, string xsd, string prefix)
        {
            _document = document;
            _xsd = xsd;
            _prefix = prefix;
            
            _elements.Push(_document.DocumentElement);
        }

        public XmlElementStack(XmlElement starting, string xsd, string prefix)
        {
            _document = starting.OwnerDocument;
            _elements.Push(starting);
            _xsd = xsd;
            _prefix = prefix;
        }

        public XmlElement Current
        {
            get { return _elements.Peek(); }
        }

        public XmlElement Push(string name, string xsd = null, string prefix = null)
        {
            var element = Add(name, xsd, prefix);

            _elements.Push(element);

            return element;
        }

        public XmlElementStack Child(string name, string xsd = null, string prefix = null)
        {
            var element = Add(name, xsd, prefix);
            return new XmlElementStack(element, xsd ?? _xsd, prefix ?? _prefix);
        }

        public void Pop()
        {
            _elements.Pop();
        }

        public XmlElement Add(string name, string xsd = null, string prefix = null)
        {
            var element = _document.CreateElement(prefix ?? _prefix, name, xsd ?? _xsd);
            Current.AppendChild(element);

            return element;
        }

        public XmlElementStack Text(string text)
        {
            Current.InnerText = text;
            return this;
        }

        public XmlElementStack Attr(string name, string value)
        {
            if (value.IsNotEmpty())
            {
                Current.Attr(name, value);
            }
            return this;
        }

        public XmlElementStack Attr(string name, DateTimeOffset value)
        {
            Current.Attr(name, XmlConvert.ToString(value));

            return this;
        }

        public XmlElementStack Attr(string name, Uri value)
        {
            if (value != null)
            {
                Current.Attr(name, value.ToString());
            }

            return this;
        }
    }
}