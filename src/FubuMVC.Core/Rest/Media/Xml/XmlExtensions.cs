using System;
using System.Xml;

namespace FubuMVC.Core.Rest.Media.Xml
{
    public static class XmlExtensions
    {

        public static XmlDocument FromFile(this XmlDocument document, string fileName)
        {
            document.Load(fileName);
            return document;
        }

        public static XmlElement WithRoot(this XmlDocument document, string elementName)
        {
            XmlElement element = document.CreateElement(elementName);
            document.AppendChild(element);

            return element;
        }

        public static XmlElement WithFormattedText(this XmlElement element, string text)
        {
            XmlCDataSection section = element.OwnerDocument.CreateCDataSection(text);
            element.AppendChild(section);

            return element;
        }

        public static XmlElement AddElement(this XmlNode element, string name)
        {
            XmlElement child = element.OwnerDocument.CreateElement(name);
            element.AppendChild(child);

            return child;
        }

        public static XmlElement AddElement(this XmlNode element, string name, Action<XmlElement> action)
        {
            XmlElement child = element.OwnerDocument.CreateElement(name);
            element.AppendChild(child);

            action(child);

            return child;
        }

    }
}