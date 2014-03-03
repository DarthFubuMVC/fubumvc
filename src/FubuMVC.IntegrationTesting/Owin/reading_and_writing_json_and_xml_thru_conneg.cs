using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class reading_and_writing_json_and_xml_thru_conneg
    {
        [Test]
        public void read_and_write_json()
        {
            HarnessApplication.Run(x => {
                var message = new Message
                {
                    Color = "Blue",
                    Direction = "East"
                };

                var response = x.PostJson(message, contentType: "text/json", accept: "text/json");

                response.StatusCodeShouldBe(HttpStatusCode.OK);
                response.ContentType.ShouldEqual("text/json");

                response.ReadAsJson<Message>().ShouldEqual(message);
            });



        }

        [Test]
        public void read_and_write_xml()
        {
            HarnessApplication.Run(x => {
                var message = new Message
                {
                    Color = "Blue",
                    Direction = "East"
                };

                var response = x.PostXml(message, contentType: "text/xml", accept: "text/xml");

                response.StatusCodeShouldBe(HttpStatusCode.OK);
                response.ContentType.ShouldEqual("text/xml");

                var serializer = new XmlSerializer(typeof(Message));
                serializer.Deserialize(new XmlTextReader(new StringReader(response.ReadAsText()))).ShouldEqual(message);
            });


        }
    }

    public class ConnegEndpoint
    {
        public Message post_message(Message message)
        {
            return message;
        }
    }

    public class Message
    {
        public string Color { get; set; }
        public string Direction { get; set; }

        public override string ToString()
        {
            return string.Format("Color: {0}, Direction: {1}", Color, Direction);
        }

        public bool Equals(Message other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Color, Color) && Equals(other.Direction, Direction);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Message)) return false;
            return Equals((Message) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Color != null ? Color.GetHashCode() : 0)*397) ^ (Direction != null ? Direction.GetHashCode() : 0);
            }
        }
    }
}