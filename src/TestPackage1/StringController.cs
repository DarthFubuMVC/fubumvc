using FubuMVC.Core;
using HtmlTags;

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

    public class ViewInput
    {
        [RouteInput]
        public string Name { get; set; }
    }

    public class ViewController
    {
        public HtmlDocument ShowView(ViewInput input)
        {
            var document = new HtmlDocument();

            document.Title = input.Name;

            document.Push("p");
            document.Add("span").Text("The input was '");
            document.Add("span").Id("name").Text(input.Name);
            document.Add("span").Text("'");

            return document;
        }
    }
}