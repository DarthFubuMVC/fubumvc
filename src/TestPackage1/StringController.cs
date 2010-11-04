using FubuMVC.Core;

namespace TestPackage1
{
    public class StringController
    {
        public string SayHello()
        {
            return "Hello";
        }
    }

    public class Message : JsonMessage
    {
        public string Name { get; set; }
    }

    public class JsonController
    {
        public Message SendMessage(Message input)
        {
            return input;
        }
    }

    public class ViewInput
    {
        [RouteInput]
        public string Name { get; set; }
    }
}