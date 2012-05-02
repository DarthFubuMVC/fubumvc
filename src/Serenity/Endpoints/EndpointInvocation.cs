using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using FubuCore;

namespace Serenity.Endpoints
{
    using FubuMVC.Core;

    public class EndpointInvocation
    {
        public string Accept = "*/*";
        public string ContentType;
        public object Target;
        public EndpointFormatting SendAs = EndpointFormatting.json;
        public string Content;
        public string Method = "POST";

        public string GetContent()
        {
            switch (SendAs)
            {
                case EndpointFormatting.json:
                    return writeJson();

                case EndpointFormatting.xml:
                    return writeXml();

                case EndpointFormatting.other:
                    return Content;

                case EndpointFormatting.form:
                    return writeForm();
            }

            return null;
        }

        private string writeForm()
        {
            var document = new XmlDocument();
            document.LoadXml(writeXml());

            var list = new List<string>();

            foreach (XmlElement element in document.DocumentElement.ChildNodes)
            {
                var data = "{0}={1}".ToFormat(element.Name, element.InnerText.UrlEncode());
                list.Add(data);
            }

            return list.Join("&");
        }


        private string writeXml()
        {
            var serializer = new XmlSerializer(Target.GetType());
            var builder = new StringBuilder();

            

            var writer = new XmlTextWriter(new StringWriter(builder));
            serializer.Serialize(writer, Target);

            return builder.ToString();
        }

        private string writeJson()
        {
            return new JavaScriptSerializer().Serialize(Target);
        }
    }
}