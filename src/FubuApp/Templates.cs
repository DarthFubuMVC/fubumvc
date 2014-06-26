using System.Threading;
using FubuMVC.Core;
using HtmlTags;

namespace FubuApp
{
    public class TemplateEndpoints
    {
        public HtmlTag Red(RedTemplate input)
        {
            return new HtmlTag("div").Text("Red: " + Thread.CurrentThread.CurrentCulture.Name);
        }

        public HtmlTag Green(GreenTemplate input)
        {
            return new HtmlTag("div").Text("Green: " + Thread.CurrentThread.CurrentCulture.Name);
        }

        public HtmlTag Blue(BlueTemplate input)
        {
            return new HtmlTag("div").Text("Blue: " + Thread.CurrentThread.CurrentCulture.Name);
        }
    }

    public class RedTemplate : Template{}
    public class GreenTemplate : Template{}
    public class BlueTemplate : Template{}
}