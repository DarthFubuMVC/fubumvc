using FubuMVC.Core;
using HtmlTags;

namespace FubuMVC.Diagnostics.Chrome
{
    public class FakeEndpoint
    {
        [WrapWith(typeof(ChromeBehavior))]
        public HtmlTag get_chrome_hello()
        {
            return new HtmlTag("h1").Text("This is content in the middle of the chrome");
        }
    }
}