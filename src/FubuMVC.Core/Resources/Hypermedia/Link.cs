namespace FubuMVC.Core.Resources.Hypermedia
{
    public class Link
    {
        public Link(string url)
        {
            Url = url;
        }

        public string Url { get; private set; }
        public string Rel { get; set; }
        public string Title { get; set; }

        public string ContentType { get; set; }
    }
}