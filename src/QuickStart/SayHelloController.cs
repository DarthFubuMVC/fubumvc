namespace QuickStart
{
    public class HelloWorldController
    {
        public string HelloWorld()
        {
            return "Hello world!";
        }
    }
}

namespace QuickStart
{
    public class HelloWorldInHtmlController
    {
        public string HelloWorld2Html()
        {
            return "<html><body><h1>Hello world!</h1></body></html>";
        }
    }
}

namespace QuickStart
{
    using HtmlTags;

    public class HelloWorldWithHtmlTagsController
    {
        public HtmlDocument BlueHello()
        {
            var document = new HtmlDocument
            {
                Title = "Saying hello to you"
            };

            document
                .Add("h1")
                .Text("Hello world!")
                .Style("color", "blue");

            return document;
        }
    }
}


namespace QuickStart
{
    using HtmlTags;

    public class SayMyNameController
    {
        public HtmlDocument get_my_name_is_Name(NameModel input)
        {
            var document = new HtmlDocument();
            document.Title = "What's your name?";
            document.Add("h1").Text("My name is " + input.Name);
            return document;
        }
    }

    public class NameModel
    {
        public string Name { get; set; }
    }
}


    /*







     */
