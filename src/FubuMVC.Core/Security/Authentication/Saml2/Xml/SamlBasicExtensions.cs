using System;
using System.Collections.Generic;
using System.Xml;
using FubuCore;
using FubuCore.Conversion;

namespace FubuMVC.Core.Security.Authentication.Saml2.Xml
{
    public static class SamlBasicExtensions
    {
        private static readonly IObjectConverter converter = new ObjectConverter();

        public static IEnumerable<XmlElement> Children(this XmlElement element, string name, string xsd)
        {
            foreach (XmlNode node in element.GetElementsByTagName(name, SamlResponseXmlReader.AssertionXsd))
            {
                yield return (XmlElement) node;
            }
        }

        public static XmlElement FindChild(this XmlElement element, string name, string xsd = SamlResponseXmlReader.AssertionXsd)
        {
            var children = element.GetElementsByTagName(name, xsd);
            return (XmlElement) (children.Count > 0 ? children[0] : null);
        }

        public static XmlElement FindChild(this XmlDocument document, string name, string xsd = SamlResponseXmlReader.AssertionXsd)
        {
            var element = document.DocumentElement;
            var children = element.GetElementsByTagName(name, xsd);
            return (XmlElement)(children.Count > 0 ? children[0] : null);
        }

        public static XmlElement EncryptedChild(this XmlDocument document, string name)
        {
            return document.FindChild(name, SamlResponseXmlReader.EncryptedXsd);
        }

        public static XmlElement EncryptedChild(this XmlElement element, string name)
        {
            return element.FindChild(name, SamlResponseXmlReader.EncryptedXsd);
        }

        public static Uri ToUri(this string uri)
        {
            if (uri.IsEmpty()) return null;

            return new Uri(uri);
        }

        public static T ToEnumValue<T>(this string text)
        {
            return (T) Enum.Parse(typeof (T), text);
        }

        public static SamlStatus ToSamlStatus(this string text)
        {
            return SamlStatus.Get(text);
        }

        public static T ReadAttribute<T>(this XmlElement element, string attribute) 
        {
            return element.HasAttribute(attribute) ? converter.FromString<T>(element.GetAttribute(attribute)) : default(T);
        }

        public static T ReadChildText<T>(this XmlElement element, string child)
        {
            var childElement = element.FindChild(child);
            if (childElement == null) return default(T);

            return converter.FromString<T>(childElement.InnerText);
        }

        public static XmlElement Attr(this XmlElement element, string name, string value)
        {
            element.SetAttribute(name, value);
            return element;
        }

        public static XmlElement Attr(this XmlElement element, string name, Uri value)
        {
            element.SetAttribute(name, value.ToString());
            return element;
        }

        public static XmlElement Attr(this XmlElement element, string name, DateTimeOffset? value)
        {
            if (value == null) return element;

            element.SetAttribute(name, XmlConvert.ToString(value.Value));
            return element;
        }

        public static XmlElement Text(this XmlElement element, string text)
        {
            element.InnerText = text;
            return element;
        }
    }
}