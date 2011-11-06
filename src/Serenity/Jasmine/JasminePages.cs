using HtmlTags;

namespace Serenity.Jasmine
{
    public class JasminePages
    {
        public HtmlDocument Home()
        {
            var document = new HtmlDocument{
                Title = "Serenity Jasmine Tester"
            };

            document.Add("h1").Text("Serenity Jasmine Tester");


            return document;
        }
    }
}