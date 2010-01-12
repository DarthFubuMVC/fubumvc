using System.Collections.Generic;
using System.Xml;

namespace FubuMVC.Core.Util
{
    public static class XmlExtensions
    {
        public static XmlElement WithRoot(this XmlDocument document, string name)
        {
            XmlElement element = document.CreateElement(name);
            document.AppendChild(element);

            return element;
        }

        public static XmlElement AddElement(this XmlNode element, string name)
        {
            XmlElement child = element.OwnerDocument.CreateElement(name);
            element.AppendChild(child);

            return child;
        }


        public static XmlElement WithAtt(this XmlElement element, string key, string attValue)
        {
            element.SetAttribute(key, attValue);
            return element;
        }

        public static XmlElement WithText(this XmlElement element, string text)
        {
            element.InnerText = text;
            return element;
        }

        public static void SetAttributeOnChild(this XmlElement element, string childName, string attName,
                                               string attValue)
        {
            XmlElement childElement = element[childName];
            if (childElement == null)
            {
                childElement = element.AddElement(childName);
            }

            childElement.SetAttribute(attName, attValue);
        }

        public static XmlElement WithProperties(this XmlElement element, IDictionary<string, string> properties)
        {
            foreach (var pair in properties)
            {
                element.SetAttribute(pair.Key, pair.Value);
            }

            return element;
        }
    }
}