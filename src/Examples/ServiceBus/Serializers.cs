using System.IO;
using Examples.HelloWorld.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;


namespace Examples.ServiceBus
{

    // SAMPLE: MyCustomSerializer
    public class MyCustomSerializer : IMessageSerializer
    {
        public void Serialize(object message, Stream stream)
        {
            // write the message object to the stream
        }

        public object Deserialize(Stream message)
        {
            // deserialize the stream into the message object
            return null;
        }

        public string ContentType { get; } = "text/special";
    }
    // ENDSAMPLE


    public class SerializationStuff
    {
        // SAMPLE: lazy-envelope-serialization
        public void ShowSerialization(Envelope envelope)
        {
            // Accessing the Message property will
            // invoke the deserialization
            var message = envelope.Message;
        }
        // ENDSAMPLE

        // SAMPLE: choose-a-content-type-when-sending
        public void SendEnvelope(IEnvelopeSender sender)
        {
            var envelope = new Envelope
            {
                Message = new PingMessage(),
                ContentType = "application/json"
            };

            sender.Send(envelope);
        }
        // ENDSAMPLE
    }

    // SAMPLE: SerializedApp
    public class SerializedApp : FubuTransportRegistry<AppSettings>
    {
        public SerializedApp()
        {
            // Register a custom message serializer
            Services.AddService<IMessageSerializer, MyCustomSerializer>();

            // Set a default serializer for the entire application
            ServiceBus.DefaultSerializer<BinarySerializer>();

            // Set a default serializer by choosing the content
            // type of an installed serializer
            ServiceBus.DefaultContentType("application/json");

            // Set the default serializer for a single channel
            Channel(x => x.Transactions)
                .DefaultSerializer<BinarySerializer>();

            // Set the default serializer for a specific channel
            // by choosing the content type of an installed serializer
            Channel(x => x.Control)
                .DefaultContentType("application/xml");
        }
    }
    // ENDSAMPLE
}