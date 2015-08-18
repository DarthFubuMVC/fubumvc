using HtmlTags;

namespace FubuMVC.IntegrationTesting
{
    public class HomeEndpoint
    {
        public HtmlDocument Index()
        {
            var document = new HtmlDocument();
            document.Title = "The home page";
            document.Add("h1").Text("The home page");

            return document;
        }

        public HtmlDocument get_different_Name(DifferentInput input)
        {
            var document = new HtmlDocument();
            document.Title = "A different page";
            document.Add("h1").Text("Page for " + input.Name);

            return document;
        }
    }

    public class DifferentInput
    {
        public string Name { get; set; }
    }
}