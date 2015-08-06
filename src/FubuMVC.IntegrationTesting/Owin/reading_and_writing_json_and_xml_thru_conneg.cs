using NUnit.Framework;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class reading_and_writing_json_and_xml_thru_conneg
    {
        [Test]
        public void read_and_write_json()
        {
            var message = new Message
            {
                Color = "Blue",
                Direction = "East"
            };

            TestHost.Scenario(_ =>
            {
                _.Post.Json(message).ContentType("text/json").Accepts("text/json");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("text/json");
            });
        }

        [Test]
        public void read_and_write_xml()
        {
            var message = new Message
            {
                Color = "Blue",
                Direction = "East"
            };

            TestHost.Scenario(_ =>
            {
                _.Post.Xml(message).Accepts("text/xml").ContentType("text/xml");
                _.ContentTypeShouldBe("text/xml");


                _.Response.Body.ReadAsXml<Message>().ShouldBe(message);
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
                return ((Color != null ? Color.GetHashCode() : 0)*397) ^
                       (Direction != null ? Direction.GetHashCode() : 0);
            }
        }
    }
}