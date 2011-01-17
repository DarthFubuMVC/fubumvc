using HtmlTags;

namespace FubuMVC.HelloWorld
{
    public class CodemashController
    {
        public HtmlDocument Hello()
        {
            var document = new HtmlDocument();
            document.Add("h1").Text("Hello!");

            return document;
        }
    }
}