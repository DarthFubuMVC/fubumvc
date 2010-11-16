using System;
using FubuMVC.Core;

namespace TestPackage1
{
    public class TestPackage1Registry : FubuPackageRegistry
    {
        public TestPackage1Registry()
        {
            Actions.IncludeClassesSuffixedWithController();

            Views.TryToAttachWithDefaultConventions();
        }

    }

    public class StringController
    {
        public string SayHello()
        {
            return "Hello";
        }
    }

    public class JsonSerializedMessage : JsonMessage
    {
        public string Name { get; set; }
    }

    public class JsonController
    {
        public JsonSerializedMessage SendMessage(JsonSerializedMessage input)
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
            return input;
        }
    }
}