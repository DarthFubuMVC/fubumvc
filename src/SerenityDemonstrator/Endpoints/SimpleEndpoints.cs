using HtmlTags;

namespace SerenityDemonstrator.Endpoints
{
    public class SimpleEndpoints
    {
        public HtmlDocument get_blue()
        {
            var document = new HtmlDocument()
            {
                Title = "Blue!"
            };

            document.Add("h1").Text("Blue");

            return document;
        }

        public string get_green()
        {
            return "the color is green";
        }

        public string post_color_Name(ColorInput input)
        {
            return "the color is " + input.Name;
        }
    }

    public class ColorInput
    {
        public string Name { get; set; }
    }
}