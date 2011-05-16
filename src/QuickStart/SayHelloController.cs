using HtmlTags;

namespace QuickStart
{
    public class HelloWorldController
    {
        public string HelloWorld()
        {
            return "Hello world!";
        }

        public string HelloWorld2Html()
        {
            return "<html><body><h1>Hello world!</h1></body></html>";
        }

        public HtmlDocument HelloAgain()
        {
            var document = new HtmlDocument{
                Title = "Saying hello to you"
            };

            document
                .Add("h1")
                .Text("Hello world!")
                .Style("color", "blue");

            return document;
        }

        public HtmlDocument get_i_am_Name(NameModel input)
        {
            var document = new HtmlDocument();
            document.Add("h1").Text("My name is " + input.Name);
            return document;
        }
    }

    public class NameModel
    {
        public string Name { get; set; }
    }
}