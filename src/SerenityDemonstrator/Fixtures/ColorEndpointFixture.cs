using System.Diagnostics;
using Serenity.Fixtures;
using SerenityDemonstrator.Endpoints;
using StoryTeller;

namespace SerenityDemonstrator.Fixtures
{
    public class ColorEndpointFixture : ScreenFixture
    {
        [FormatAs("Fetch /blue")]
        public void FetchBlue()
        {
            var text = Endpoints.Get<SimpleEndpoints>(x => x.get_blue())
                .ReadAsText();

            Debug.WriteLine("Got from /blue:");
            Debug.WriteLine(text);
        }

        [FormatAs("Fetch /green")]
        public void FetchGreen()
        {
            var text = Endpoints.Get<SimpleEndpoints>(x => x.get_green())
                .ReadAsText();

            Debug.WriteLine("Got from /green:");
            Debug.WriteLine(text);
        }

        [FormatAs("Fetching /color/{color} should return text {text}")]
        public void FetchColor(string color)
        {
            var text = Endpoints.PostJson(new ColorInput {Name = color})
                .ReadAsText();

            Debug.WriteLine("Got from /color/" + color + ":");
            Debug.WriteLine(text);
        }

    }
}