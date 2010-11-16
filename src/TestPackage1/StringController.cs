using System;
using FubuMVC.Core;

namespace TestPackage1
{
    public class TestPackage1Registry : FubuPackageRegistry
    {
        public TestPackage1Registry()
        {
            Actions.IncludeClassesSuffixedWithController();
        }

    }

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

    public class ViewController
    {
        public ViewInput ShowView(ViewInput input)
        {
            return new ViewInput();
        }
    }
}