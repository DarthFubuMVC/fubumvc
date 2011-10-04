using FubuCore;

namespace FubuMVC.Core.Rest
{
    public class Link
    {
        public Link(string url)
        {
            Url = url.ToAbsoluteUrl();
        }

        public string Url { get; private set; }
        public string Rel { get; set; }
        public string Title { get; set; }

        public string ContentType { get; set; }
    }
}