using HtmlTags;

namespace FubuMVC.IntegrationTesting
{

    public class DifferentEndpoint
    {
        public HtmlDocument get_different_Name(DifferentInput input)
        {
            var document = new HtmlDocument();
            document.Title = "A different page";
            document.Add("h1").Text("Page for " + input.Name);

            return document;
        }
    }
}