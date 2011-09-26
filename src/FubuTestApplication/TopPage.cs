using HtmlTags;

namespace FubuTestApplication
{
    public class TopPage
    {
        public HtmlDocument Welcome()
        {
            var document = new HtmlDocument();
            document.Title = "Fubu Testing Application";
            document.Add("h1").Text("Welcome to the FubuMVC Testing Application!");

            return document;
        }

        public HtmlDocument get_my_name_is_Name(MyNameIsInput input)
        {
            var document = new HtmlDocument();

            document.Add("h1").Text("My name is " + input.Name);

            return document;
        }
    }

    public class MyNameIsInput
    {
        public string Name { get; set;}
    }
}