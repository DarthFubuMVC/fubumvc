using FubuMVC.Core;

namespace TestPackage2
{
    public class TestPackage2Registry : FubuPackageRegistry
    {
        public TestPackage2Registry()
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

    public class JsonSerializedMessage 
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