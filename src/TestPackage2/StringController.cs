using System;
using FubuMVC.Core;

namespace TestPackage2
{
    public class TestPackage2Registry : FubuPackageRegistry
    {
        public TestPackage2Registry()
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

}